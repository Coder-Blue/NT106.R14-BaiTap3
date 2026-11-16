using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using Microsoft.Data.SqlClient;
using NT106.R14.BaiTap3.Shared;

namespace NT106.R14.BaiTap3.Server;

public sealed class MayChu
{
    private readonly QuanLyPhien _phien = new();
    private readonly KhoaDangNhap _boKhoaDangNhap = new();
    private readonly ConcurrentDictionary<string, TcpClient> _clients = new();

    private TcpListener? _listener;
    private CancellationTokenSource? _nguonHuy;
    private Task? _vongLapNhan;
    private Task? _udpTask;
    private volatile bool _dangChay;

    public bool DangChay => _dangChay;

    public event Action<string>? CoLog;

    public IReadOnlyList<ClientDangKetNoi> LayDanhSachClient() => _phien.LayDanhSach();

    public async Task<bool> BatDauAsync()
    {
        if (_dangChay)
        {
            return true;
        }

        try
        {
            await XuLyNguoiDung.KhoiTaoAsync();
            GhiLog("Đã kiểm tra cơ sở dữ liệu.");
        }
        catch (SqlException)
        {
            GhiLog("[CSDL] Không khởi tạo được cơ sở dữ liệu.");
        }
        catch (InvalidOperationException)
        {
            GhiLog("[CSDL] Cấu hình kết nối không hợp lệ.");
        }
        catch (ArgumentException)
        {
            GhiLog("[CSDL] Chuỗi kết nối không hợp lệ.");
        }

        _phien.XoaTatCa();
        _clients.Clear();
        _nguonHuy = new CancellationTokenSource();
        _listener = new TcpListener(IPAddress.Any, GiaoThuc.CongMacDinh);

        try
        {
            _listener.Start();
            _dangChay = true;
            GhiLog($"Server đang nghe tại {IPAddress.Any}:{GiaoThuc.CongMacDinh}.");
            _vongLapNhan = Task.Run(() => VongLapNhanAsync(_nguonHuy.Token));
            _udpTask = Task.Run(() => LangNgheUdpAsync(_nguonHuy.Token));
            return true;
        }
        catch (SocketException)
        {
            _dangChay = false;
            _listener.Stop();
            _listener = null;
            _nguonHuy.Dispose();
            _nguonHuy = null;
            GhiLog($"[LỖI] Không mở được cổng {GiaoThuc.CongMacDinh}. Cổng có thể đang được sử dụng.");
            return false;
        }
    }

    public async Task DungAsync()
    {
        if (!_dangChay)
        {
            return;
        }

        _dangChay = false;
        _nguonHuy?.Cancel();
        _listener?.Stop();
        DongTatCaClient();

        Task? vongLap = _vongLapNhan;
        Task? udp = _udpTask;
        _vongLapNhan = null;
        _udpTask = null;

        if (vongLap is not null)
        {
            try
            {
                await vongLap;
            }
            catch (OperationCanceledException)
            {
            }
        }

        if (udp is not null)
        {
            try
            {
                await udp;
            }
            catch (OperationCanceledException)
            {
            }
        }

        DongTatCaClient();
        _nguonHuy?.Dispose();
        _nguonHuy = null;
        _listener = null;
        _phien.XoaTatCa();
        GhiLog("Server đã dừng.");
    }

    public void DungNhanh()
    {
        if (!_dangChay)
        {
            return;
        }

        _dangChay = false;
        _nguonHuy?.Cancel();
        _listener?.Stop();
        DongTatCaClient();
        _phien.XoaTatCa();
        _nguonHuy?.Dispose();
        _nguonHuy = null;
        _listener = null;
    }

    private void DongTatCaClient()
    {
        foreach (TcpClient client in _clients.Values)
        {
            try
            {
                client.Close();
            }
            catch (ObjectDisposedException)
            {
            }
            catch (SocketException)
            {
            }
        }

        _clients.Clear();
    }

    private async Task VongLapNhanAsync(CancellationToken token)
    {
        try
        {
            while (!token.IsCancellationRequested)
            {
                TcpClient client = await _listener!.AcceptTcpClientAsync(token);
                _ = PhucVuClientAsync(client, token);
            }
        }
        catch (OperationCanceledException)
        {
        }
        catch (ObjectDisposedException)
        {
        }
        catch (SocketException)
        {
            if (!token.IsCancellationRequested)
            {
                GhiLog("[LỖI] Lỗi socket khi chờ kết nối.");
            }
        }
        finally
        {
            _listener?.Stop();
        }
    }

    private async Task PhucVuClientAsync(TcpClient client, CancellationToken token)
    {
        string maPhien = client.Client.RemoteEndPoint?.ToString() ?? Guid.NewGuid().ToString();
        string ip = (client.Client.RemoteEndPoint as IPEndPoint)?.Address.ToString() ?? "khong-ro";
        _phien.ThemKetNoi(maPhien);
        _clients[maPhien] = client;
        GhiLog($"[+] Kết nối mới từ {maPhien} | Đang hoạt động: {_phien.SoPhien}");

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
                    PhienDangNhap? phien = _phien.TimPhien(maPhien, yeuCau.Token);
                    if (phien is null)
                    {
                        var loiToken = new PhanHoi(false, GiaoThuc.LoiHetHanToken, "Phiên đăng nhập không hợp lệ hoặc đã hết hạn.");
                        await boGhi.WriteLineAsync(GiaoThuc.DongGoi(loiToken));
                        continue;
                    }

                    _phien.XoaPhien(maPhien);
                    GhiLog($"[-] Client {maPhien} ({phien.TenDangNhap}) đăng xuất. Còn lại: {_phien.SoPhien}");

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
        catch (IOException)
        {
            GhiLog($"[!] Client {maPhien} mất kết nối.");
        }
        catch (SocketException)
        {
            GhiLog($"[!] Lỗi socket tại {maPhien}.");
        }
        catch (ObjectDisposedException)
        {
        }
        catch (InvalidOperationException)
        {
            GhiLog($"[!] Lỗi xử lý client {maPhien}.");
        }
        // Lưới an toàn cuối cùng cho tác vụ nền; không hiển thị chi tiết lỗi cho người dùng.
        catch (Exception)
        {
            GhiLog($"[!] Lỗi không mong đợi tại {maPhien}.");
        }
        finally
        {
            _clients.TryRemove(maPhien, out _);
            PhienDangNhap? phienConLai = _phien.XoaPhien(maPhien);
            if (phienConLai is not null)
            {
                GhiLog($"[-] Đã gỡ phiên {maPhien} ({phienConLai.TenDangNhap}). Còn lại: {_phien.SoPhien}");
            }

            client.Close();
        }
    }

    private async Task<PhanHoi> XuLyYeuCauAsync(string maPhien, string ip, YeuCau yeuCau)
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
        catch (SqlException)
        {
            GhiLog("[LỖI CSDL] Không xử lý được yêu cầu.");
            return new PhanHoi(false, GiaoThuc.LoiHeThong, "Server không xử lý được yêu cầu. Vui lòng thử lại.");
        }
        catch (InvalidOperationException)
        {
            GhiLog("[LỖI KẾT NỐI CSDL] Không xử lý được yêu cầu.");
            return new PhanHoi(false, GiaoThuc.LoiHeThong, "Server không xử lý được yêu cầu. Vui lòng thử lại.");
        }
    }

    private async Task<PhanHoi> DangKyAsync(YeuCau yeuCau)
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

    private async Task<PhanHoi> DangNhapAsync(string maPhien, string ip, YeuCau yeuCau)
    {
        string tenDangNhap = yeuCau.TenDangNhap.Trim();

        if (tenDangNhap.Length == 0 || yeuCau.MatKhau.Length == 0)
        {
            return new PhanHoi(false, GiaoThuc.LoiSaiThongTin, "Tên đăng nhập hoặc mật khẩu không đúng.");
        }

        if (_boKhoaDangNhap.ThoiGianConKhoa(tenDangNhap, ip) is TimeSpan conKhoa)
        {
            GhiLog($"[KHÓA] Từ chối đăng nhập: {ip} ({tenDangNhap}), còn {(int)conKhoa.TotalSeconds}s");
            return PhanHoiTamKhoa(conKhoa);
        }

        NguoiDung? nguoiDung = await XuLyNguoiDung.DangNhap(tenDangNhap, yeuCau.MatKhau);
        if (nguoiDung is null)
        {
            (int soLanSai, TimeSpan? khoaTrong) = _boKhoaDangNhap.GhiNhanSai(tenDangNhap, ip);
            GhiLog($"[ĐĂNG NHẬP] Thất bại lần {soLanSai}/{KhoaDangNhap.SoLanSaiToiDa}: {ip} ({tenDangNhap})");

            if (khoaTrong is TimeSpan thoiGianKhoa)
            {
                GhiLog($"[KHÓA] Tạm khóa {tenDangNhap} từ {ip} trong {(int)thoiGianKhoa.TotalSeconds}s");
                return PhanHoiTamKhoa(thoiGianKhoa);
            }

            return new PhanHoi(false, GiaoThuc.LoiSaiThongTin, "Tên đăng nhập hoặc mật khẩu không đúng.");
        }

        _boKhoaDangNhap.XoaKhiThanhCong(tenDangNhap, ip);

        PhienDangNhap phien = _phien.TaoPhien(nguoiDung.Username);
        _phien.DatPhien(maPhien, phien);

        GhiLog($"[ĐĂNG NHẬP] Thành công: {maPhien} -> {nguoiDung.Username}");
        return new PhanHoi(true, GiaoThuc.ThanhCong, "Đăng nhập thành công.", nguoiDung, phien.Token, phien.HetHan);
    }

    private static PhanHoi PhanHoiTamKhoa(TimeSpan conLai)
    {
        int giay = (int)Math.Ceiling(conLai.TotalSeconds);
        string cho = giay >= 60 ? $"{giay / 60} phút {giay % 60} giây" : $"{giay} giây";
        return new PhanHoi(false, GiaoThuc.LoiTamKhoa, $"Bạn đã đăng nhập sai quá nhiều lần. Vui lòng thử lại sau {cho}.");
    }

    private async Task<PhanHoi> LayThongTinAsync(string maPhien, YeuCau yeuCau)
    {
        PhienDangNhap? phien = _phien.TimPhien(maPhien, yeuCau.Token);
        if (phien is null)
        {
            return new PhanHoi(false, GiaoThuc.LoiHetHanToken, "Phiên đăng nhập không hợp lệ hoặc đã hết hạn.");
        }

        NguoiDung? nguoiDung = await XuLyNguoiDung.LayThongTin(phien.TenDangNhap);
        return nguoiDung is null
            ? new PhanHoi(false, GiaoThuc.LoiSaiThongTin, "Không tìm thấy người dùng.")
            : new PhanHoi(true, GiaoThuc.ThanhCong, "OK", nguoiDung);
    }

    private PhanHoi GiaHanToken(string maPhien, YeuCau yeuCau)
    {
        PhienDangNhap? phienMoi = _phien.GiaHanPhien(maPhien, yeuCau.Token);
        if (phienMoi is null)
        {
            GhiLog($"[TOKEN] Gia hạn thất bại: {maPhien}");
            return new PhanHoi(false, GiaoThuc.LoiHetHanToken, "Phiên đăng nhập không hợp lệ hoặc đã hết hạn.");
        }

        GhiLog($"[GIA HẠN] {maPhien} ({phienMoi.TenDangNhap}) đến {phienMoi.HetHan:dd/MM/yyyy HH:mm:ss} UTC");
        return new PhanHoi(true, GiaoThuc.ThanhCong, "Gia hạn phiên thành công.", null, phienMoi.Token, phienMoi.HetHan);
    }

    private async Task LangNgheUdpAsync(CancellationToken token)
    {
        try
        {
            using var udpServer = new UdpClient(GiaoThuc.CongUdpDiscovery);
            GhiLog($"[UDP] Đang lắng nghe dò tìm tại cổng {GiaoThuc.CongUdpDiscovery}...");

            while (!token.IsCancellationRequested)
            {
                UdpReceiveResult result = await udpServer.ReceiveAsync(token);
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
        catch (ObjectDisposedException)
        {
        }
        catch (SocketException)
        {
            if (!token.IsCancellationRequested)
            {
                GhiLog("[UDP] Không mở được cổng dò tìm.");
            }
        }
        // Lưới an toàn cuối cùng cho tác vụ nền; không hiển thị chi tiết lỗi cho người dùng.
        catch (Exception)
        {
            GhiLog("[UDP] Lỗi không mong đợi.");
        }
    }

    private void GhiLog(string noiDung) =>
        CoLog?.Invoke($"[{DateTime.Now:dd/MM/yyyy HH:mm:ss}] {noiDung}");
}
