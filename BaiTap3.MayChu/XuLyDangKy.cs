using BaiTap3.Chung;
using Microsoft.Data.SqlClient;

namespace BaiTap3.MayChu;

/// <summary>Xử lý lệnh DANGKY. Được gọi từ phần phân tích lệnh trong MayChuTcp.</summary>
public static class XuLyDangKy
{
    public static async Task<PhanHoi> XuLyAsync(YeuCau yeuCau, CancellationToken token, Action<string>? ghiNhatKy = null)
    {
        var tenDangNhap = yeuCau.LayThamSo(GiaoThuc.Khoa.TenDangNhap).Trim();
        var matKhau = yeuCau.LayThamSo(GiaoThuc.Khoa.MatKhau);
        var hoTen = yeuCau.LayThamSo(GiaoThuc.Khoa.HoTen).Trim();
        var email = yeuCau.LayThamSo(GiaoThuc.Khoa.Email).Trim();

        // Không tin client: kiểm tra lại bằng cùng bộ luật.
        var loiNhap = KiemTraNhapLieu.TenDangNhap(tenDangNhap)
                      ?? KiemTraNhapLieu.MatKhau(matKhau)
                      ?? KiemTraNhapLieu.HoTen(hoTen)
                      ?? KiemTraNhapLieu.Email(email);
        if (loiNhap is not null)
        {
            ghiNhatKy?.Invoke("DANGKY bị từ chối: dữ liệu không hợp lệ.");
            return PhanHoi.TaoLoi(MaLoi.ThamSoKhongHopLe, loiNhap);
        }

        try
        {
            var ketQua = await XuLyNguoiDung.DangKyAsync(tenDangNhap, matKhau, hoTen, email, token);
            if (ketQua == KetQuaDangKy.TenDaTonTai)
            {
                ghiNhatKy?.Invoke($"DANGKY thất bại: tên '{tenDangNhap}' đã tồn tại.");
                return PhanHoi.TaoLoi(MaLoi.TenDaTonTai, "Tên này đã có người dùng.");
            }

            ghiNhatKy?.Invoke($"DANGKY thành công: '{tenDangNhap}'.");
            return PhanHoi.TaoThanhCong("Đăng ký thành công.");
        }
        catch (SqlException ex)
        {
            ghiNhatKy?.Invoke($"Lỗi cơ sở dữ liệu (mã {ex.Number}): {ex.Message}");
            return PhanHoi.TaoLoi(MaLoi.LoiCoSoDuLieu, "Máy chủ không truy cập được cơ sở dữ liệu. Vui lòng thử lại sau.");
        }
    }
}
