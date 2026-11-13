using System.IO;
using System.Net.Sockets;
using System.Text;

namespace CRUD_APP;

public sealed class KetNoiTcp : IDisposable
{
    private TcpClient? _client;
    private StreamReader? _reader;
    private StreamWriter? _writer;

    public bool DangKetNoi => _client is { Connected: true };

    public async Task KetNoiAsync(string ip, int cong)
    {
        if (DangKetNoi) return;

        _client = new TcpClient();
        await _client.ConnectAsync(ip, cong);
        NetworkStream stream = _client.GetStream();
        _reader = new StreamReader(stream, Encoding.UTF8);
        _writer = new StreamWriter(stream, new UTF8Encoding(false)) { AutoFlush = true };
    }

    public async Task<PhanHoi> GuiYeuCauAsync(YeuCau yeuCau)
    {
        if (!DangKetNoi || _writer is null || _reader is null)
            throw new SocketException((int)SocketError.NotConnected);

        string jsonGoi = GiaoThuc.DongGoi(yeuCau);
        await _writer.WriteLineAsync(jsonGoi);

        string? jsonNhan = await _reader.ReadLineAsync();
        if (jsonNhan is null)
            throw new IOException("Server đã đóng kết nối.");

        return GiaoThuc.GiaiMa<PhanHoi>(jsonNhan)
               ?? new PhanHoi(false, GiaoThuc.LoiHeThong, "Phản hồi từ server không hợp lệ.");
    }

    public void GuiNhanhDangXuatVaDong()
    {
        try
        {
            if (DangKetNoi && _writer is not null)
            {
                string json = GiaoThuc.DongGoi(new YeuCau(GiaoThuc.LenhDangXuat));
                _writer.WriteLine(json);
            }
        }
        catch (SocketException) { }
        catch (IOException) { }
        finally
        {
            Dispose();
        }
    }

    public void Dispose()
    {
        try { _writer?.Dispose(); } catch { }
        try { _reader?.Dispose(); } catch { }
        try { _client?.Close(); } catch { }
        _writer = null;
        _reader = null;
        _client = null;
    }
}
