using System.Text.Json;

namespace CRUD_APP;

public static class GiaoThuc
{
    public const int CongMacDinh = 8888;

    public const string LenhDangKy = "DANGKY";
    public const string LenhDangNhap = "DANGNHAP";
    public const string LenhLayThongTin = "LAYTHONGTIN";
    public const string LenhDangXuat = "DANGXUAT";

    public const string ThanhCong = "OK";
    public const string LoiTrungTen = "ERR_TRUNG_TEN";
    public const string LoiSaiThongTin = "ERR_SAI_THONG_TIN";
    public const string LoiDuLieuKhongHopLe = "ERR_DU_LIEU";
    public const string LoiHeThong = "ERR_HE_THONG";

    private static readonly JsonSerializerOptions TuyenChonJson = new() { WriteIndented = false };

    public static string DongGoi<T>(T goiTin) => JsonSerializer.Serialize(goiTin, TuyenChonJson);

    public static T? GiaiMa<T>(string dongJson) => JsonSerializer.Deserialize<T>(dongJson, TuyenChonJson);
}

public record YeuCau(
    string MaLenh,
    string TenDangNhap = "",
    string MatKhau = "",
    string HoTen = "",
    string Email = ""
);

public record PhanHoi(
    bool ThanhCong,
    string MaLoi,
    string ThongBao,
    NguoiDung? DuLieuNguoiDung = null
);
