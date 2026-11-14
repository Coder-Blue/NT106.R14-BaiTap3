namespace BaiTap3.Chung;

/// <summary>Hằng số của giao thức, dùng chung cho client và server.</summary>
public static class GiaoThuc
{
    // Cổng nằm trong dải 1024–49151 theo yêu cầu đề bài.
    public const int CongMacDinh = 5050;

    public static class Lenh
    {
        public const string DangKy = "DANGKY";
        public const string DangNhap = "DANGNHAP";
        public const string LayThongTin = "LAYTHONGTIN";
        public const string DangXuat = "DANGXUAT";
    }

    /// <summary>Tên các khoá trong phần tham số của yêu cầu.</summary>
    public static class Khoa
    {
        public const string TenDangNhap = "tenDangNhap";
        public const string MatKhau = "matKhau";
        public const string HoTen = "hoTen";
        public const string Email = "email";
    }
}

/// <summary>Mã lỗi trong phản hồi. 0 là thành công.</summary>
public static class MaLoi
{
    public const int ThanhCong = 0;
    public const int LenhKhongHopLe = 100;
    public const int ThamSoKhongHopLe = 101;
    public const int TenDaTonTai = 200;
    public const int LoiCoSoDuLieu = 300;
    public const int LoiMayChu = 500;
}
