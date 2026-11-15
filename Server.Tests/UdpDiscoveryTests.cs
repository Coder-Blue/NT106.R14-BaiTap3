using NT106.R14.BaiTap3.Server;
using NT106.R14.BaiTap3.Shared;

namespace Server.Tests;

public class UdpDiscoveryTests
{
    [Fact]
    public void LenhDung_TraVeCongTcp()
    {
        Assert.Equal(GiaoThuc.CongMacDinh.ToString(), UdpDiscovery.TaoPhanHoi(GiaoThuc.LenhDoTimServer));
    }

    [Fact]
    public void LenhKhac_KhongTraLoi()
    {
        Assert.Null(UdpDiscovery.TaoPhanHoi("KHONG_PHAI_DISCOVER"));
    }
}
