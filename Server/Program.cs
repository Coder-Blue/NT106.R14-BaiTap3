using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using Microsoft.Data.SqlClient;
using NT106.R14.BaiTap3.Shared;

namespace NT106.R14.BaiTap3.Server;

internal static class Program
{
    private static readonly ConcurrentDictionary<string, PhienDangNhap> DanhSachPhien = new();
    private static readonly TimeSpan ThoiHanPhien = TimeSpan.FromMinutes(30);

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

        var listener = new TcpListener(IPAddress.Any, GiaoThuc.CongMacDinh);

        try
        {
            listener.Start();
            GhiLog($"Server đang nghe tại {IPAddress.Any}:{GiaoThuc.CongMacDinh}. Nhấn Ctrl+C để dừng.");

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
        DanhSachPhien[maPhien] = PhienDangNhap.ChuaDangNhap;
        GhiLog($"[+] Kết nối mới từ {maPhien} | Đang hoạt động: {DanhSachPhien.Count}");

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
                    PhienDangNhap? phien = TimPhien(maPhien, yeuCau.Token);
                    if (phien is null)
                    {
                        var loiToken = new PhanHoi(false, GiaoThuc.LoiHetHanToken, "Phiên đăng nhập không hợp lệ hoặc đã hết hạn.");
                        await boGhi.WriteLineAsync(GiaoThuc.DongGoi(loiToken));
                        continue;
                    }

                    DanhSachPhien.TryRemove(maPhien, out _);
                    GhiLog($"[-] Client {maPhien} ({phien.TenDangNhap}) đăng xuất. Còn lại: {DanhSachPhien.Count}");

                    var phanHoiDangXuat = new PhanHoi(true, GiaoThuc.ThanhCong, "Đã đăng xuất.");
                    await boGhi.WriteLineAsync(GiaoThuc.DongGoi(phanHoiDangXuat));
                    break;
                }

                PhanHoi phanHoi = await XuLyYeuCauAsync(maPhien, yeuCau);
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
            if (DanhSachPhien.TryRemove(maPhien, out PhienDangNhap? phienConLai))
            {
                GhiLog($"[-] Đã gỡ phiên {maPhien} ({phienConLai.TenDangNhap}). Còn lại: {DanhSachPhien.Count}");
            }

            client.Close();
        }
    }

    private static async Task<PhanHoi> XuLyYeuCauAsync(string maPhien, YeuCau yeuCau)
    {
        try
        {
            return yeuCau.MaLenh switch
            {
                GiaoThuc.LenhDangKy => await DangKyAsync(yeuCau),
                GiaoThuc.LenhDangNhap => await DangNhapAsync(maPhien, yeuCau),
                GiaoThuc.LenhLayThongTin => await LayThongTinAsync(maPhien, yeuCau),
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

    private static async Task<PhanHoi> DangNhapAsync(string maPhien, YeuCau yeuCau)
    {
        string tenDangNhap = yeuCau.TenDangNhap.Trim();

        if (tenDangNhap.Length == 0 || yeuCau.MatKhau.Length == 0)
        {
            return new PhanHoi(false, GiaoThuc.LoiSaiThongTin, "Tên đăng nhập hoặc mật khẩu không đúng.");
        }

        NguoiDung? nguoiDung = await XuLyNguoiDung.DangNhap(tenDangNhap, yeuCau.MatKhau);
        if (nguoiDung is null)
        {
            GhiLog($"[ĐĂNG NHẬP] Thất bại: {maPhien} ({tenDangNhap})");
            return new PhanHoi(false, GiaoThuc.LoiSaiThongTin, "Tên đăng nhập hoặc mật khẩu không đúng.");
        }

        string token = MatKhau.TaoToken();
        var phien = new PhienDangNhap(nguoiDung.Username, token, DateTime.UtcNow.Add(ThoiHanPhien));
        DanhSachPhien[maPhien] = phien;

        GhiLog($"[ĐĂNG NHẬP] Thành công: {maPhien} -> {nguoiDung.Username}");
        return new PhanHoi(true, GiaoThuc.ThanhCong, "Đăng nhập thành công.", nguoiDung, token, phien.HetHan);
    }

    private static async Task<PhanHoi> LayThongTinAsync(string maPhien, YeuCau yeuCau)
    {
        PhienDangNhap? phien = TimPhien(maPhien, yeuCau.Token);
        if (phien is null)
        {
            return new PhanHoi(false, GiaoThuc.LoiHetHanToken, "Phiên đăng nhập không hợp lệ hoặc đã hết hạn.");
        }

        NguoiDung? nguoiDung = await XuLyNguoiDung.LayThongTin(phien.TenDangNhap);
        return nguoiDung is null
            ? new PhanHoi(false, GiaoThuc.LoiSaiThongTin, "Không tìm thấy người dùng.")
            : new PhanHoi(true, GiaoThuc.ThanhCong, "OK", nguoiDung);
    }

    private static PhienDangNhap? TimPhien(string maPhien, string token)
    {
        if (token.Length == 0 || !DanhSachPhien.TryGetValue(maPhien, out PhienDangNhap? phien))
        {
            return null;
        }

        if (phien.Token != token || phien.HetHan <= DateTime.UtcNow)
        {
            if (phien.Token.Length > 0)
            {
                GhiLog($"[TOKEN] Token không hợp lệ hoặc đã hết hạn: {maPhien}");
            }

            DanhSachPhien.TryRemove(maPhien, out _);
            return null;
        }

        return phien;
    }

    private static void GhiLog(string noiDung) =>
        Console.WriteLine($"[{DateTime.Now:dd/MM/yyyy HH:mm:ss}] {noiDung}");
}
