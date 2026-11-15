using NT106.R14.BaiTap3.Server;

namespace Server.Tests;

public class KhoaDangNhapTests
{
    private static void Sai(KhoaDangNhap k, int soLan, string ten = "an", string ip = "1.1.1.1")
    {
        for (int i = 0; i < soLan; i++)
        {
            k.GhiNhanSai(ten, ip);
        }
    }

    [Fact]
    public void BonLanSai_ChuaKhoa()
    {
        var k = new KhoaDangNhap(new DongHoGia());
        Sai(k, 4);
        Assert.Null(k.ThoiGianConKhoa("an", "1.1.1.1"));
    }

    [Fact]
    public void NamLanSai_BiKhoa()
    {
        var k = new KhoaDangNhap(new DongHoGia());
        Sai(k, 5);
        Assert.NotNull(k.ThoiGianConKhoa("an", "1.1.1.1"));
    }

    [Fact]
    public void HetThoiGianKhoa_DuocMo()
    {
        var dongHo = new DongHoGia();
        var k = new KhoaDangNhap(dongHo);
        Sai(k, 5);
        dongHo.Tien(TimeSpan.FromMinutes(5) + TimeSpan.FromSeconds(1));
        Assert.Null(k.ThoiGianConKhoa("an", "1.1.1.1"));
    }

    [Fact]
    public void DangNhapDung_XoaBoDem()
    {
        var k = new KhoaDangNhap(new DongHoGia());
        Sai(k, 4);
        k.XoaKhiThanhCong("an", "1.1.1.1");
        Sai(k, 4);
        Assert.Null(k.ThoiGianConKhoa("an", "1.1.1.1"));
    }

    [Fact]
    public void IpKhac_KhongBiKhoa()
    {
        var k = new KhoaDangNhap(new DongHoGia());
        Sai(k, 5);
        Assert.Null(k.ThoiGianConKhoa("an", "2.2.2.2"));
    }

    [Fact]
    public void SaiNgoaiCuaSo_DemLai()
    {
        var dongHo = new DongHoGia();
        var k = new KhoaDangNhap(dongHo);
        Sai(k, 4);
        dongHo.Tien(TimeSpan.FromMinutes(6));
        Sai(k, 4);
        Assert.Null(k.ThoiGianConKhoa("an", "1.1.1.1"));
    }
}
