namespace NT106.R14.BaiTap3.Shared;

public record NguoiDung(
    string TenDangNhap,
    string HoTen,
    string Email,
    DateTime NgayTao,
    DateTime? LanDangNhapCuoi);
