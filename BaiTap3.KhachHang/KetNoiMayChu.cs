using System.Net.Sockets;
using System.Text.Json;
using BaiTap3.Chung;

namespace BaiTap3.KhachHang;

/// <summary>Một kết nối TCP tới server: mở, gửi yêu cầu, nhận phản hồi (đóng khung theo KhungThongDiep).</summary>
public sealed class KetNoiMayChu : IDisposable
{
    private readonly TcpClient _client = new();
    private NetworkStream? _luong;

    /// <exception cref="SocketException">Không nối được.</exception>
    /// <exception cref="OperationCanceledException">Hết thời gian chờ hoặc bị huỷ.</exception>
    public async Task KetNoiAsync(string diaChi, int cong, CancellationToken token)
    {
        await _client.ConnectAsync(diaChi, cong, token);
        _luong = _client.GetStream();
    }

    /// <exception cref="IOException">Mất kết nối khi đang gửi/nhận.</exception>
    /// <exception cref="InvalidDataException">Khung thông điệp sai.</exception>
    /// <exception cref="JsonException">Phản hồi không phải JSON hợp lệ.</exception>
    public async Task<PhanHoi> GuiAsync(YeuCau yeuCau, CancellationToken token)
    {
        if (_luong is null)
            throw new IOException("Chưa kết nối.");

        await KhungThongDiep.GhiAsync(_luong, BoMaHoa.DongGoi(yeuCau), token);

        var duLieu = await KhungThongDiep.DocAsync(_luong, token)
                     ?? throw new IOException("Máy chủ đã đóng kết nối.");
        return BoMaHoa.MoGoi<PhanHoi>(duLieu) ?? throw new JsonException("Phản hồi rỗng.");
    }

    public void Dispose() => _client.Dispose();
}
