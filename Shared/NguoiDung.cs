namespace CRUD_APP;

public record NguoiDung(
    string TenDangNhap,
    string HoTen,
    string Email,
    DateTime NgayTao,
    DateTime? LanDangNhapCuoi);
