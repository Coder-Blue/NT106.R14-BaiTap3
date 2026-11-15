using NT106.R14.BaiTap3.Server;

namespace Server.Tests;

public class QuanLyPhienTests
{
    private static QuanLyPhien TaoQuanLy(DongHoGia dongHo, params string[] tokens)
    {
        int chiSo = 0;
        return new QuanLyPhien(dongHo, () => tokens[chiSo++]);
    }

    [Fact]
    public void TaoPhien_TokenMoiVaHetHan30Phut()
    {
        var dongHo = new DongHoGia();
        var quanLy = TaoQuanLy(dongHo, "token-1");

        var phien = quanLy.TaoPhien("an");

        Assert.Equal("token-1", phien.Token);
        Assert.Equal(dongHo.GetUtcNow().UtcDateTime.AddMinutes(30), phien.HetHan);
    }

    [Fact]
    public void GiaHan_XoayTokenVaCong30Phut()
    {
        var dongHo = new DongHoGia();
        var quanLy = TaoQuanLy(dongHo, "token-1", "token-2");
        var phien = quanLy.TaoPhien("an");
        quanLy.DatPhien("conn", phien);
        dongHo.Tien(TimeSpan.FromMinutes(25));

        var phienMoi = quanLy.GiaHanPhien("conn", "token-1");

        Assert.NotNull(phienMoi);
        Assert.Equal("token-2", phienMoi!.Token);
        Assert.Equal(phien.HetHan.AddMinutes(30), phienMoi.HetHan);
        Assert.NotNull(quanLy.TimPhien("conn", "token-2"));
    }

    [Fact]
    public void GiaHan_TokenCuKhongConHieuLuc()
    {
        var dongHo = new DongHoGia();
        var quanLy = TaoQuanLy(dongHo, "token-1", "token-2");
        quanLy.DatPhien("conn", quanLy.TaoPhien("an"));
        dongHo.Tien(TimeSpan.FromMinutes(25));
        Assert.NotNull(quanLy.GiaHanPhien("conn", "token-1"));

        Assert.Null(quanLy.GiaHanPhien("conn", "token-1"));
    }

    [Fact]
    public void GiaHan_TokenSai_TraNull()
    {
        var dongHo = new DongHoGia();
        var quanLy = TaoQuanLy(dongHo, "token-1");
        quanLy.DatPhien("conn", quanLy.TaoPhien("an"));

        Assert.Null(quanLy.GiaHanPhien("conn", "khong-dung"));
    }

    [Fact]
    public void PhienHetHan_KhongTimThay()
    {
        var dongHo = new DongHoGia();
        var quanLy = TaoQuanLy(dongHo, "token-1");
        quanLy.DatPhien("conn", quanLy.TaoPhien("an"));
        dongHo.Tien(TimeSpan.FromMinutes(31));

        Assert.Null(quanLy.TimPhien("conn", "token-1"));
    }

    [Fact]
    public void PhienHetHan_KhongGiaHanDuoc()
    {
        var dongHo = new DongHoGia();
        var quanLy = TaoQuanLy(dongHo, "token-1");
        quanLy.DatPhien("conn", quanLy.TaoPhien("an"));
        dongHo.Tien(TimeSpan.FromMinutes(31));

        Assert.Null(quanLy.GiaHanPhien("conn", "token-1"));
    }

    [Fact]
    public void XoaPhien_TraVePhienDaXoa()
    {
        var dongHo = new DongHoGia();
        var quanLy = TaoQuanLy(dongHo, "token-1");
        var phien = quanLy.TaoPhien("an");
        quanLy.DatPhien("conn", phien);

        Assert.Equal(phien, quanLy.XoaPhien("conn"));
        Assert.Null(quanLy.XoaPhien("conn"));
    }
}
