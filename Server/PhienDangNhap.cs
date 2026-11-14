namespace NT106.R14.BaiTap3.Server;

public sealed record PhienDangNhap(string TenDangNhap, string Token, DateTime HetHan)
{
    public static readonly PhienDangNhap ChuaDangNhap = new("(chưa đăng nhập)", "", DateTime.MinValue);
}
