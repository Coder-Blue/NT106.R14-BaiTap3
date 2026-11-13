using System.Data;
using Microsoft.Data.SqlClient;

namespace CRUD_APP;

public static class XuLyNguoiDung
{
    // Đổi tên instance cho đúng với máy cài SQL Server nếu cần.
    private const string ChuoiKetNoi =
        @"Server=(localdb)\MSSQLLocalDB;Database=QuanLyNguoiDung;Trusted_Connection=True;TrustServerCertificate=True;";

    private const string LoiKetNoi = "Không thể kết nối đến cơ sở dữ liệu. Vui lòng kiểm tra lại SQL Server.";

    public static async Task<bool> KiemTraTenDangNhapTonTai(string tenDangNhap)
    {
        const string sql = "SELECT COUNT(*) FROM Users WHERE TenDangNhap = @ten";
        try
        {
            await using var conn = new SqlConnection(ChuoiKetNoi);
            await conn.OpenAsync();

            await using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.Add("@ten", SqlDbType.NVarChar, 20).Value = tenDangNhap;
            return Convert.ToInt32(await cmd.ExecuteScalarAsync()) > 0;
        }
        catch (Exception ex)
        {
            //throw new Exception(LoiKetNoi, ex);
            throw new Exception($"{LoiKetNoi}\nChi tiết lỗi: {ex.Message}", ex);
        }
    }

    public static async Task DangKy(string tenDangNhap, string matKhau, string hoTen, string email)
    {
        var (salt, bam) = MatKhau.Tao(matKhau);

        const string sql = @"INSERT INTO Users (TenDangNhap, MatKhauBam, Salt, HoTen, Email)
                             VALUES (@ten, @bam, @salt, @hoTen, @email)";
        try
        {
            await using var conn = new SqlConnection(ChuoiKetNoi);
            await conn.OpenAsync();

            await using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.Add("@ten", SqlDbType.NVarChar, 20).Value = tenDangNhap;
            cmd.Parameters.Add("@bam", SqlDbType.NVarChar, 64).Value = bam;
            cmd.Parameters.Add("@salt", SqlDbType.NVarChar, 32).Value = salt;
            cmd.Parameters.Add("@hoTen", SqlDbType.NVarChar, 50).Value = hoTen;
            cmd.Parameters.Add("@email", SqlDbType.NVarChar, 100).Value = email.Length == 0 ? DBNull.Value : email;
            await cmd.ExecuteNonQueryAsync();
        }
        catch (SqlException ex) when (ex.Number is 2627 or 2601)
        {
            // Ràng buộc UNIQUE trên TenDangNhap là mức chặn trùng thứ hai nếu kiểm tra phía chương trình bị bỏ qua.
            throw new Exception("Tên này đã có người dùng.", ex);
        }
        catch (Exception ex)
        {
            //throw new Exception(LoiKetNoi, ex);
            throw new Exception($"{LoiKetNoi}\nChi tiết lỗi: {ex.Message}", ex);
        }
    }

    public static async Task<NguoiDung?> DangNhap(string tenDangNhap, string matKhau)
    {
        const string sql = @"SELECT MatKhauBam, Salt, HoTen, Email, NgayTao, LanDangNhapCuoi
                             FROM Users WHERE TenDangNhap = @ten";
        try
        {
            await using var conn = new SqlConnection(ChuoiKetNoi);
            await conn.OpenAsync();

            NguoiDung nguoiDung;
            await using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.Add("@ten", SqlDbType.NVarChar, 20).Value = tenDangNhap;
                await using var reader = await cmd.ExecuteReaderAsync();

                // Sai tên hoặc sai mật khẩu đều trả về null để form hiển thị một thông báo duy nhất.
                if (!await reader.ReadAsync() || !MatKhau.KiemTra(matKhau, reader.GetString(1), reader.GetString(0)))
                {
                    return null;
                }

                nguoiDung = new NguoiDung(
                    tenDangNhap,
                    reader.GetString(2),
                    reader.IsDBNull(3) ? "" : reader.GetString(3),
                    reader.GetDateTime(4),
                    reader.IsDBNull(5) ? null : reader.GetDateTime(5));
            }

            const string sqlCapNhat = "UPDATE Users SET LanDangNhapCuoi = SYSDATETIME() WHERE TenDangNhap = @ten";
            await using (var cmd = new SqlCommand(sqlCapNhat, conn))
            {
                cmd.Parameters.Add("@ten", SqlDbType.NVarChar, 20).Value = tenDangNhap;
                await cmd.ExecuteNonQueryAsync();
            }

            return nguoiDung;
        }
        catch (Exception ex)
        {
            //throw new Exception(LoiKetNoi, ex);
            throw new Exception($"{LoiKetNoi}\nChi tiết lỗi: {ex.Message}", ex);
        }
    }

    public static async Task<NguoiDung?> LayThongTin(string tenDangNhap)
    {
        const string sql = @"SELECT HoTen, Email, NgayTao, LanDangNhapCuoi
                         FROM Users WHERE TenDangNhap = @ten";
        try
        {
            await using var conn = new SqlConnection(ChuoiKetNoi);
            await conn.OpenAsync();
            await using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.Add("@ten", SqlDbType.NVarChar, 20).Value = tenDangNhap;
            await using var reader = await cmd.ExecuteReaderAsync();

            if (!await reader.ReadAsync()) return null;

            return new NguoiDung(
                tenDangNhap,
                reader.GetString(0),
                reader.IsDBNull(1) ? "" : reader.GetString(1),
                reader.GetDateTime(2),
                reader.IsDBNull(3) ? null : reader.GetDateTime(4));
        }
        catch (Exception ex)
        {
            throw new Exception(LoiKetNoi, ex);
        }
    }
}
