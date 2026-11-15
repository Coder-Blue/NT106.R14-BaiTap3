using NT106.R14.BaiTap3.Shared;

namespace NT106.R14.BaiTap3.Server;

public static class UdpDiscovery
{
    public static string? TaoPhanHoi(string yeuCau) =>
        yeuCau == GiaoThuc.LenhDoTimServer ? GiaoThuc.CongMacDinh.ToString() : null;
}
