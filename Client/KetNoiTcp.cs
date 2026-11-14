using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using NT106.R14.BaiTap3.Shared;

namespace NT106.R14.BaiTap3.Client;

public sealed class KetNoiTcp : IDisposable
{
    private TcpClient? client;
    private StreamReader? boDoc;
    private StreamWriter? boGhi;

    public bool DangKetNoi() => client is { Connected: true } && boDoc is not null && boGhi is not null;

    public async Task KetNoiAsync(string ip, int cong)
    {
        if (DangKetNoi())
        {
            return;
        }

        Dispose();

        var tcp = new TcpClient();
        using var hetGio = new CancellationTokenSource(TimeSpan.FromSeconds(10));

        try
        {
            await tcp.ConnectAsync(ip, cong, hetGio.Token);
        }
        catch
        {
            tcp.Dispose();
            throw;
        }

        client = tcp;
        NetworkStream stream = tcp.GetStream();
        boDoc = new StreamReader(stream, new UTF8Encoding(false), false, 4096, leaveOpen: true);
        boGhi = new StreamWriter(stream, new UTF8Encoding(false), 4096, leaveOpen: true)
        {
            AutoFlush = true,
            NewLine = "\n",
        };
    }

    public async Task<PhanHoi> GuiYeuCauAsync(YeuCau yeuCau)
    {
        if (!DangKetNoi() || boDoc is null || boGhi is null)
        {
            throw new SocketException((int)SocketError.NotConnected);
        }

        using var hetGio = new CancellationTokenSource(TimeSpan.FromSeconds(20));
        string jsonGoi = GiaoThuc.DongGoi(yeuCau);
        await boGhi.WriteLineAsync(jsonGoi.AsMemory(), hetGio.Token);

        string? jsonNhan = await boDoc.ReadLineAsync(hetGio.Token);
        if (jsonNhan is null)
        {
            throw new IOException("Server đã đóng kết nối.");
        }

        try
        {
            return GiaoThuc.GiaiMa<PhanHoi>(jsonNhan)
                ?? new PhanHoi(false, GiaoThuc.LoiHeThong, "Phản hồi từ server không hợp lệ.");
        }
        catch (JsonException)
        {
            return new PhanHoi(false, GiaoThuc.LoiHeThong, "Phản hồi từ server không hợp lệ.");
        }
    }

    public void GuiNhanhVaDong(YeuCau yeuCau)
    {
        try
        {
            if (DangKetNoi() && boGhi is not null)
            {
                boGhi.WriteLine(GiaoThuc.DongGoi(yeuCau));
            }
        }
        catch (Exception ex) when (ex is IOException or SocketException or ObjectDisposedException)
        {
        }
        finally
        {
            Dispose();
        }
    }

    public void Dispose()
    {
        DongBoGhi();
        boGhi = null;
        boDoc?.Dispose();
        boDoc = null;
        client?.Close();
        client = null;
    }

    private void DongBoGhi()
    {
        try
        {
            boGhi?.Dispose();
        }
        catch (Exception ex) when (ex is IOException or ObjectDisposedException)
        {
        }
    }
}
