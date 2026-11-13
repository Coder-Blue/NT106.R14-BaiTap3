using System.Text.RegularExpressions;

namespace CRUD_APP;

public static class KiemTraNhapLieu
{
    private static readonly Regex DinhDangEmail = new(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");

    public static string? TenDangNhap(string tenDangNhap)
    {
        if (tenDangNhap.Length == 0) return "Không được để trống.";
        if (tenDangNhap.Length is < 3 or > 20) return "Tên đăng nhập phải có từ 3 đến 20 ký tự.";
        return null;
    }

    public static string? MatKhau(string matKhau)
    {
        if (matKhau.Length == 0) return "Không được để trống.";
        if (matKhau.Length < 6) return "Mật khẩu phải có ít nhất 6 ký tự.";
        return null;
    }

    public static string? XacNhanMatKhau(string matKhau, string xacNhan)
    {
        if (xacNhan.Length == 0) return "Không được để trống.";
        if (xacNhan != matKhau) return "Mật khẩu xác nhận không khớp.";
        return null;
    }

    public static string? HoTen(string hoTen)
    {
        if (hoTen.Length == 0) return "Không được để trống.";
        if (hoTen.Length > 50) return "Họ tên tối đa 50 ký tự.";
        return null;
    }

    public static string? Email(string email)
    {
        if (email.Length == 0) return null;
        if (email.Length > 100) return "Email tối đa 100 ký tự.";
        return DinhDangEmail.IsMatch(email) ? null : "Địa chỉ thư điện tử không đúng định dạng.";
    }
}
