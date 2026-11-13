using System.Collections.Concurrent;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using CRUD_APP;

namespace Server;

internal static class Program
{
    private static readonly ConcurrentDictionary<string, string> DanhSachPhien = new();

    private static async Task Main()
    {
        Console.OutputEncoding = Encoding.UTF8;
        using var cts = new CancellationTokenSource();

        Console.CancelKeyPress += (_, e) =>
        {
            e.Cancel = true;
            GhiLog("Đang nhận tín hiệu dừng Server...");
            cts.Cancel();
        };

        var listener = new TcpListener(IPAddress.Any, GiaoThuc.CongMacDinh);

        try
        {
            listener.Start();
            GhiLog($"Server đã khởi động tại cổng {GiaoThuc.CongMacDinh}. Nhấn Ctrl+C để dừng.");

            while (!cts.Token.IsCancellationRequested)
            {
                TcpClient client = await listener.AcceptTcpClientAsync(cts.Token);
                _ = PhucVuClientAsync(client, cts.Token);
            }
        }
        catch (OperationCanceledException)
        {
        }
        catch (SocketException ex)
        {
            GhiLog($"Lỗi khởi động Socket trên Server: {ex.Message}");
        }
        finally
        {
            listener.Stop();
            GhiLog("Server đã dừng hoàn toàn.");
        }
    }

    private static async Task PhucVuClientAsync(TcpClient client, CancellationToken token)
    {
        string maPhien = client.Client.RemoteEndPoint?.ToString() ?? Guid.NewGuid().ToString();
        DanhSachPhien.TryAdd(maPhien, "(chưa đăng nhập)");
        GhiLog($"[+] Kết nối mới từ {maPhien} | Đang hoạt động: {DanhSachPhien.Count}");

        try
        {
            await using NetworkStream stream = client.GetStream();
            using var reader = new StreamReader(stream, Encoding.UTF8);
            await using var writer = new StreamWriter(stream, new UTF8Encoding(false)) { AutoFlush = true };

            while (!token.IsCancellationRequested)
            {
                string? dongJson = await reader.ReadLineAsync(token);

                if (dongJson is null)
                {
                    GhiLog($"[!] Client {maPhien} đã đóng cửa sổ kết nối.");
                    break;
                }

                YeuCau? yeuCau = GiaoThuc.GiaiMa<YeuCau>(dongJson);
                if (yeuCau is null) continue;

                if (yeuCau.MaLenh == GiaoThuc.LenhDangXuat)
                {
                    if (DanhSachPhien.TryRemove(maPhien, out string? userDangXuat))
                    {
                        GhiLog($"[-] Client {maPhien} (User: {userDangXuat}) gửi lệnh DANGXUAT. Đã gỡ phiên. | Còn lại: {DanhSachPhien.Count}");
                    }
                    var phanHoiDx = new PhanHoi(true, GiaoThuc.ThanhCong, "Đã đăng xuất thành công.");
                    await writer.WriteLineAsync(GiaoThuc.DongGoi(phanHoiDx));
                    break; 
                }

                PhanHoi phanHoi = await XuLyYeuCauAsync(maPhien, yeuCau);
                await writer.WriteLineAsync(GiaoThuc.DongGoi(phanHoi));
            }
        }
        catch (OperationCanceledException) { }
        catch (IOException ex)
        {
            GhiLog($"[!] Client {maPhien} mất kết nối đột ngột (IOException): {ex.Message}");
        }
        catch (SocketException ex)
        {
            GhiLog($"[!] Lỗi Socket tại Client {maPhien}: {ex.Message}");
        }
        finally
        {
            if (DanhSachPhien.TryRemove(maPhien, out string? userConLai))
            {
                GhiLog($"[-] Đã gỡ phiên {maPhien} (User: {userConLai}) khỏi danh sách. | Còn lại: {DanhSachPhien.Count}");
            }
            client.Close();
        }
    }

    private static async Task<PhanHoi> XuLyYeuCauAsync(string maPhien, YeuCau yc)
    {
        try
        {
            switch (yc.MaLenh)
            {
                case GiaoThuc.LenhDangKy:
                    if (await XuLyNguoiDung.KiemTraTenDangNhapTonTai(yc.TenDangNhap))
                    {
                        GhiLog($"[ĐĂNG KÝ THẤT BẠI] Trùng tên đăng nhập: {yc.TenDangNhap}");
                        return new PhanHoi(false, GiaoThuc.LoiTrungTen, "Tên này đã có người dùng.");
                    }
                    await XuLyNguoiDung.DangKy(yc.TenDangNhap, yc.MatKhau, yc.HoTen, yc.Email);
                    GhiLog($"[ĐĂNG KÝ THÀNH CÔNG] Tài khoản mới: {yc.TenDangNhap}");
                    return new PhanHoi(true, GiaoThuc.ThanhCong, "Đăng ký thành công! Bạn có thể đăng nhập ngay.");

                case GiaoThuc.LenhDangNhap:
                    NguoiDung? nd = await XuLyNguoiDung.DangNhap(yc.TenDangNhap, yc.MatKhau);
                    if (nd is null)
                    {
                        GhiLog($"[ĐĂNG NHẬP THẤT BẠI] Phiên {maPhien} nhập sai thông tin ({yc.TenDangNhap}).");
                        return new PhanHoi(false, GiaoThuc.LoiSaiThongTin, "Tên đăng nhập hoặc mật khẩu không đúng.");
                    }
                    DanhSachPhien[maPhien] = nd.TenDangNhap;
                    GhiLog($"[ĐĂNG NHẬP THÀNH CÔNG] Phiên {maPhien} -> User: {nd.TenDangNhap}");
                    return new PhanHoi(true, GiaoThuc.ThanhCong, "Đăng nhập thành công.", nd);

                case GiaoThuc.LenhLayThongTin:
                    NguoiDung? thongTin = await XuLyNguoiDung.LayThongTin(yc.TenDangNhap);
                    return thongTin is not null
                        ? new PhanHoi(true, GiaoThuc.ThanhCong, "OK", thongTin)
                        : new PhanHoi(false, GiaoThuc.LoiSaiThongTin, "Không tìm thấy người dùng.");

                default:
                    return new PhanHoi(false, GiaoThuc.LoiDuLieuKhongHopLe, "Lệnh không hợp lệ.");
            }
        }
        catch (Exception ex)
        {
            GhiLog($"[LỖI CSDL] {ex.Message}");
            return new PhanHoi(false, GiaoThuc.LoiHeThong, ex.Message);
        }
    }

    private static void GhiLog(string noiDung)
    {
        Console.WriteLine($"[{DateTime.Now:dd/MM/yyyy HH:mm:ss}] {noiDung}");
    }
}
