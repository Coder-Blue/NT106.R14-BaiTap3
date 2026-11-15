using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using Microsoft.Data.SqlClient;
using NT106.R14.BaiTap3.Shared;

namespace NT106.R14.BaiTap3.Server;

internal static class Program
{
    private static readonly QuanLyPhien Phien = new();
    private static readonly KhoaDangNhap BoKhoaDangNhap = new();

    private static async Task Main()
    {
        Console.OutputEncoding = Encoding.UTF8;
        using var nguonHuy = new CancellationTokenSource();

        Console.CancelKeyPress += (_, suKien) =>
        {
            suKien.Cancel = true;
            GhiLog("Đang dừng server...");
            nguonHuy.Cancel();
        };

        try
        {
            await XuLyNguoiDung.KhoiTaoAsync();
            GhiLog("Đã kiểm tra cơ sở dữ liệu.");
        }
        catch (SqlException ex)
        {
            GhiLog($"[CSDL] Không khởi tạo được: {ex.Message}");
        }
        catch (InvalidOperationException ex)
        {
            GhiLog($"[CSDL] Cấu hình kết nối không hợp lệ: {ex.Message}");
        }
        catch (ArgumentException ex)
        {
            GhiLog($"[CSDL] Chuỗi kết nối không hợp lệ: {ex.Message}");
        }

        var listener = new TcpListener(IPAddress.Any, GiaoThuc.CongMacDinh);

        try
        {
            listener.Start();
            GhiLog($"Server đang nghe tại {IPAddress.Any}:{GiaoThuc.CongMacDinh}. Nhấn Ctrl+C để dừng.");
            _ = Task.Run(() => LangNgheUdpDiscovery(nguonHuy.Token));

            while (!nguonHuy.Token.IsCancellationRequested)
            {
                TcpClient client = await listener.AcceptTcpClientAsync(nguonHuy.Token);
                _ = PhucVuClientAsync(client, nguonHuy.Token);
            }
        }
        catch (OperationCanceledException)
        {
        }
        catch (SocketException ex)
        {
            GhiLog($"Lỗi socket khi khởi động server: {ex.Message}");
        }
        finally
        {
            listener.Stop();
            GhiLog("Server đã dừng.");
        }
    }

    private static async Task PhucVuClientAsync(TcpClient client, CancellationToken token)
    {
        string maPhien = client.Client.RemoteEndPoint?.ToString() ?? Guid.NewGuid().ToString();
        string ip = (client.Client.RemoteEndPoint as IPEndPoint)?.Address.ToString() ?? "khong-ro";
        Phien.ThemKetNoi(maPhien);
        GhiLog($"[+] Kết nối mới từ {maPhien} | Đang hoạt động: {Phien.SoPhien}");

        try
        {
            await using NetworkStream stream = client.GetStream();
            using var boDoc = new StreamReader(stream, new UTF8Encoding(false), false, 4096, leaveOpen: true);
            await using var boGhi = new StreamWriter(stream, new UTF8Encoding(false), 4096, leaveOpen: true)
            {
                AutoFlush = true,
                NewLine = "\n",
            };

            while (!token.IsCancellationRequested)
            {
                string? dongJson = await boDoc.ReadLineAsync(token);
                if (dongJson is null)
                {
                    GhiLog($"[!] Client {maPhien} đã đóng kết nối.");
                    break;
                }

                YeuCau? yeuCau;
                try
                {
                    yeuCau = GiaoThuc.GiaiMa<YeuCau>(dongJson);
                }
                catch (JsonException)
                {
                    var loiJson = new PhanHoi(false, GiaoThuc.LoiDuLieuKhongHopLe, "Dữ liệu gửi lên không hợp lệ.");
                    await boGhi.WriteLineAsync(GiaoThuc.DongGoi(loiJson));
                    continue;
                }

                if (yeuCau is null)
                {
                    var loi = new PhanHoi(false, GiaoThuc.LoiDuLieuKhongHopLe, "Dữ liệu gửi lên không hợp lệ.");
                    await boGhi.WriteLineAsync(GiaoThuc.DongGoi(loi));
                    continue;
                }

                if (yeuCau.MaLenh == GiaoThuc.LenhDangXuat)
                {
                    PhienDangNhap? phien = Phien.TimPhien(maPhien, yeuCau.Token);
                    if (phien is null)
                    {
                        var loiToken = new PhanHoi(false, GiaoThuc.LoiHetHanToken, "Phiên đăng nhập không hợp lệ hoặc đã hết hạn.");
                        await boGhi.WriteLineAsync(GiaoThuc.DongGoi(loiToken));
                        continue;
                    }

                    Phien.XoaPhien(maPhien);
                    GhiLog($"[-] Client {maPhien} ({phien.TenDangNhap}) đăng xuất. Còn lại: {Phien.SoPhien}");

                    var phanHoiDangXuat = new PhanHoi(true, GiaoThuc.ThanhCong, "Đã đăng xuất.");
                    await boGhi.WriteLineAsync(GiaoThuc.DongGoi(phanHoiDangXuat));
                    break;
                }

                PhanHoi phanHoi = await XuLyYeuCauAsync(maPhien, ip, yeuCau);
                await boGhi.WriteLineAsync(GiaoThuc.DongGoi(phanHoi));
            }
        }
        catch (OperationCanceledException) when (token.IsCancellationRequested)
        {
        }
        catch (IOException ex)
        {
            GhiLog($"[!] Client {maPhien} mất kết nối: {ex.Message}");
        }
        catch (SocketException ex)
        {
            GhiLog($"[!] Lỗi socket tại {maPhien}: {ex.Message}");
        }
        catch (Exception ex)
        {
            GhiLog($"[!] Lỗi không xác định tại {maPhien}: {ex.Message}");
        }
        finally
        {
            PhienDangNhap? phienConLai = Phien.XoaPhien(maPhien);
            if (phienConLai is not null)
            {
                GhiLog($"[-] Đã gỡ phiên {maPhien} ({phienConLai.TenDangNhap}). Còn lại: {Phien.SoPhien}");
            }

            client.Close();
        }
    }

    private static async Task<PhanHoi> XuLyYeuCauAsync(string maPhien, string ip, YeuCau yeuCau)
    {
        try
        {
            return yeuCau.MaLenh switch
            {
                GiaoThuc.LenhDangKy => await DangKyAsync(yeuCau),
                GiaoThuc.LenhDangNhap => await DangNhapAsync(maPhien, ip, yeuCau),
                GiaoThuc.LenhLayThongTin => await LayThongTinAsync(maPhien, yeuCau),
                GiaoThuc.LenhGiaHanToken => GiaHanToken(maPhien, yeuCau),
                _ => new PhanHoi(false, GiaoThuc.LoiDuLieuKhongHopLe, "Lệnh không hợp lệ."),
            };
        }
        catch (SqlException ex)
        {
            GhiLog($"[LỖI CSDL] {ex.Message}");
            return new PhanHoi(false, GiaoThuc.LoiHeThong, "Server không xử lý được yêu cầu. Vui lòng thử lại.");
        }
        catch (InvalidOperationException ex)
        {
            GhiLog($"[LỖI KẾT NỐI CSDL] {ex.Message}");
            return new PhanHoi(false, GiaoThuc.LoiHeThong, "Server không xử lý được yêu cầu. Vui lòng thử lại.");
        }
    }

    private static async Task<PhanHoi> DangKyAsync(YeuCau yeuCau)
    {
        string tenDangNhap = yeuCau.TenDangNhap.Trim();
        string hoTen = yeuCau.HoTen.Trim();
        string email = yeuCau.Email.Trim();

        string? loi = KiemTraNhapLieu.TenDangNhap(tenDangNhap)
            ?? KiemTraNhapLieu.MatKhau(yeuCau.MatKhau)
            ?? KiemTraNhapLieu.HoTen(hoTen)
            ?? KiemTraNhapLieu.Email(email)
            ?? KiemTraNhapLieu.NgaySinh(yeuCau.Birthday);

        if (loi is not null)
        {
            return new PhanHoi(false, GiaoThuc.LoiDuLieuKhongHopLe, loi);
        }

        if (await XuLyNguoiDung.KiemTraTenDangNhapTonTai(tenDangNhap))
        {
            GhiLog($"[ĐĂNG KÝ] Trùng tên đăng nhập: {tenDangNhap}");
            return new PhanHoi(false, GiaoThuc.LoiTrungTen, "Tên đăng nhập này đã được sử dụng.");
        }

        try
        {
            await XuLyNguoiDung.DangKy(tenDangNhap, yeuCau.MatKhau, hoTen, email, yeuCau.Birthday!.Value.Date);
        }
        catch (SqlException ex) when (ex.Number is 2627 or 2601)
        {
            GhiLog($"[ĐĂNG KÝ] Trùng tên đăng nhập: {tenDangNhap}");
            return new PhanHoi(false, GiaoThuc.LoiTrungTen, "Tên đăng nhập này đã được sử dụng.");
        }

        GhiLog($"[ĐĂNG KÝ] Thành công: {tenDangNhap}");
        return new PhanHoi(true, GiaoThuc.ThanhCong, "Đăng ký thành công. Bạn có thể đăng nhập ngay.");
    }

    private static async Task<PhanHoi> DangNhapAsync(string maPhien, string ip, YeuCau yeuCau)
    {
        string tenDangNhap = yeuCau.TenDangNhap.Trim();

        if (tenDangNhap.Length == 0 || yeuCau.MatKhau.Length == 0)
        {
            return new PhanHoi(false, GiaoThuc.LoiSaiThongTin, "Tên đăng nhập hoặc mật khẩu không đúng.");
        }

        if (BoKhoaDangNhap.ThoiGianConKhoa(tenDangNhap, ip) is TimeSpan conKhoa)
        {
            GhiLog($"[KHÓA] Từ chối đăng nhập: {ip} ({tenDangNhap}), còn {(int)conKhoa.TotalSeconds}s");
            return PhanHoiTamKhoa(conKhoa);
        }

        NguoiDung? nguoiDung = await XuLyNguoiDung.DangNhap(tenDangNhap, yeuCau.MatKhau);
        if (nguoiDung is null)
        {
            (int soLanSai, TimeSpan? khoaTrong) = BoKhoaDangNhap.GhiNhanSai(tenDangNhap, ip);
            GhiLog($"[ĐĂNG NHẬP] Thất bại lần {soLanSai}/{KhoaDangNhap.SoLanSaiToiDa}: {ip} ({tenDangNhap})");

            if (khoaTrong is TimeSpan thoiGianKhoa)
            {
                GhiLog($"[KHÓA] Tạm khóa {tenDangNhap} từ {ip} trong {(int)thoiGianKhoa.TotalSeconds}s");
                return PhanHoiTamKhoa(thoiGianKhoa);
            }

            return new PhanHoi(false, GiaoThuc.LoiSaiThongTin, "Tên đăng nhập hoặc mật khẩu không đúng.");
        }

        BoKhoaDangNhap.XoaKhiThanhCong(tenDangNhap, ip);

        PhienDangNhap phien = Phien.TaoPhien(nguoiDung.Username);
        Phien.DatPhien(maPhien, phien);

        GhiLog($"[ĐĂNG NHẬP] Thành công: {maPhien} -> {nguoiDung.Username}");
        return new PhanHoi(true, GiaoThuc.ThanhCong, "Đăng nhập thành công.", nguoiDung, phien.Token, phien.HetHan);
    }

    private static PhanHoi PhanHoiTamKhoa(TimeSpan conLai)
    {
        int giay = (int)Math.Ceiling(conLai.TotalSeconds);
        string cho = giay >= 60 ? $"{giay / 60} phút {giay % 60} giây" : $"{giay} giây";
        return new PhanHoi(false, GiaoThuc.LoiTamKhoa, $"Bạn đã đăng nhập sai quá nhiều lần. Vui lòng thử lại sau {cho}.");
    }

    private static async Task<PhanHoi> LayThongTinAsync(string maPhien, YeuCau yeuCau)
    {
        PhienDangNhap? phien = Phien.TimPhien(maPhien, yeuCau.Token);
        if (phien is null)
        {
            return new PhanHoi(false, GiaoThuc.LoiHetHanToken, "Phiên đăng nhập không hợp lệ hoặc đã hết hạn.");
        }

        NguoiDung? nguoiDung = await XuLyNguoiDung.LayThongTin(phien.TenDangNhap);
        return nguoiDung is null
            ? new PhanHoi(false, GiaoThuc.LoiSaiThongTin, "Không tìm thấy người dùng.")
            : new PhanHoi(true, GiaoThuc.ThanhCong, "OK", nguoiDung);
    }

    private static PhanHoi GiaHanToken(string maPhien, YeuCau yeuCau)
    {
        PhienDangNhap? phienMoi = Phien.GiaHanPhien(maPhien, yeuCau.Token);
        if (phienMoi is null)
        {
            GhiLog($"[TOKEN] Gia hạn thất bại: {maPhien}");
            return new PhanHoi(false, GiaoThuc.LoiHetHanToken, "Phiên đăng nhập không hợp lệ hoặc đã hết hạn.");
        }

        GhiLog($"[GIA HẠN] {maPhien} ({phienMoi.TenDangNhap}) đến {phienMoi.HetHan:dd/MM/yyyy HH:mm:ss} UTC");
        return new PhanHoi(true, GiaoThuc.ThanhCong, "Gia hạn phiên thành công.", null, phienMoi.Token, phienMoi.HetHan);
    }

    private static async Task LangNgheUdpDiscovery(CancellationToken token)
    {
        try
        {
            using var udpServer = new UdpClient(GiaoThuc.CongUdpDiscovery);
            GhiLog($"[UDP] Đang lắng nghe dò tìm tại cổng {GiaoThuc.CongUdpDiscovery}...");

            while (!token.IsCancellationRequested)
            {
                var result = await udpServer.ReceiveAsync(token);
                string yeuCau = Encoding.UTF8.GetString(result.Buffer);
                string? phanHoi = UdpDiscovery.TaoPhanHoi(yeuCau);

                if (phanHoi is not null)
                {
                    byte[] data = Encoding.UTF8.GetBytes(phanHoi);
                    await udpServer.SendAsync(data.AsMemory(), result.RemoteEndPoint, token);
                    GhiLog($"[UDP] Đã trả lời dò tìm cho {result.RemoteEndPoint}");
                }
            }
        }
        catch (OperationCanceledException)
        {
        }
        catch (Exception ex)
        {
            GhiLog($"[UDP] Lỗi: {ex.Message}");
        }
    }

    private static void GhiLog(string noiDung) =>
        Console.WriteLine($"[{DateTime.Now:dd/MM/yyyy HH:mm:ss}] {noiDung}");
}
