using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Text.Json;
using BaiTap3.Chung;

namespace BaiTap3.MayChu;

public sealed class MayChuTcp
{
    // Quá thời gian này không nhận được gì thì coi như client mất mạng đột ngột và gỡ phiên.
    private static readonly TimeSpan ThoiGianChoToiDa = TimeSpan.FromMinutes(5);

    private readonly ConcurrentDictionary<Guid, TcpClient> _khachDangNoi = new();

    public async Task ChayAsync(int cong, CancellationToken token)
    {
        var listener = new TcpListener(IPAddress.Any, cong);
        try
        {
            listener.Start();
            NhatKy.Ghi($"Máy chủ đang nghe ở cổng {cong}.");

            while (!token.IsCancellationRequested)
            {
                var client = await listener.AcceptTcpClientAsync(token);
                _ = PhucVuAsync(client, token); // cố ý không await để quay lại nghe ngay
            }
        }
        catch (OperationCanceledException)
        {
            // Dừng chủ động bằng CancellationToken.
        }
        catch (SocketException ex)
        {
            NhatKy.Ghi($"Lỗi socket khi lắng nghe: {ex.SocketErrorCode}.");
        }
        finally
        {
            listener.Stop();
            NhatKy.Ghi("Máy chủ đã dừng.");
        }
    }

    private async Task PhucVuAsync(TcpClient client, CancellationToken token)
    {
        var id = Guid.NewGuid();
        var diaChi = client.Client.RemoteEndPoint?.ToString() ?? "không rõ";
        _khachDangNoi[id] = client;
        NhatKy.Ghi($"[{diaChi}] Kết nối mới. Đang có {_khachDangNoi.Count} client.");

        try
        {
            using var dongKetNoi = client;
            var luong = client.GetStream();

            while (!token.IsCancellationRequested)
            {
                byte[]? duLieu;
                using (var cts = CancellationTokenSource.CreateLinkedTokenSource(token))
                {
                    cts.CancelAfter(ThoiGianChoToiDa);
                    duLieu = await KhungThongDiep.DocAsync(luong, cts.Token);
                }

                if (duLieu is null)
                {
                    NhatKy.Ghi($"[{diaChi}] Client đã đóng kết nối.");
                    break;
                }

                var phanHoi = await XuLyYeuCauAsync(duLieu, diaChi, token);
                await KhungThongDiep.GhiAsync(luong, BoMaHoa.DongGoi(phanHoi), token);
            }
        }
        catch (OperationCanceledException) when (token.IsCancellationRequested)
        {
            NhatKy.Ghi($"[{diaChi}] Ngắt vì máy chủ đang dừng.");
        }
        catch (OperationCanceledException)
        {
            NhatKy.Ghi($"[{diaChi}] Quá {ThoiGianChoToiDa.TotalMinutes:0} phút không có dữ liệu, ngắt kết nối.");
        }
        catch (IOException ex) // gồm cả EndOfStreamException và lỗi socket bị bọc
        {
            NhatKy.Ghi($"[{diaChi}] Mất kết nối đột ngột: {ex.Message}");
        }
        catch (SocketException ex)
        {
            NhatKy.Ghi($"[{diaChi}] Lỗi socket: {ex.SocketErrorCode}.");
        }
        catch (InvalidDataException ex)
        {
            NhatKy.Ghi($"[{diaChi}] Sai khung thông điệp: {ex.Message}");
        }
        catch (Exception ex)
        {
            // Chốt chặn cuối: hàm chạy nền mà ném ngoại lệ không ai bắt thì server im lặng.
            NhatKy.Ghi($"[{diaChi}] Lỗi không lường trước: {ex}");
        }
        finally
        {
            _khachDangNoi.TryRemove(id, out _);
            NhatKy.Ghi($"[{diaChi}] Đã gỡ khỏi danh sách. Còn {_khachDangNoi.Count} client.");
        }
    }

    private static async Task<PhanHoi> XuLyYeuCauAsync(byte[] duLieu, string diaChi, CancellationToken token)
    {
        YeuCau? yeuCau;
        try
        {
            yeuCau = BoMaHoa.MoGoi<YeuCau>(duLieu);
        }
        catch (JsonException)
        {
            yeuCau = null;
        }

        if (yeuCau is null || string.IsNullOrWhiteSpace(yeuCau.Lenh))
        {
            NhatKy.Ghi($"[{diaChi}] Yêu cầu không đọc được.");
            return PhanHoi.TaoLoi(MaLoi.LenhKhongHopLe, "Yêu cầu không hợp lệ.");
        }

        NhatKy.Ghi($"[{diaChi}] Nhận lệnh {yeuCau.Lenh}.");
        return yeuCau.Lenh switch
        {
            GiaoThuc.Lenh.DangKy => await XuLyDangKy.XuLyAsync(yeuCau, token, s => NhatKy.Ghi($"[{diaChi}] {s}")),
            // DANGNHAP, LAYTHONGTIN, DANGXUAT: thêm nhánh tương ứng khi cài đặt.
            _ => PhanHoi.TaoLoi(MaLoi.LenhKhongHopLe, "Lệnh không được hỗ trợ."),
        };
    }
}
