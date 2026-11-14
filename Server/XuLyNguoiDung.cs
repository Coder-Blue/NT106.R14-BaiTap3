using System.Data;
using Microsoft.Data.SqlClient;
using NT106.R14.BaiTap3.Shared;

namespace NT106.R14.BaiTap3.Server;

public static class XuLyNguoiDung
{
    private static readonly string ChuoiKetNoi =
        Environment.GetEnvironmentVariable("BAITAP3_CONNECTION")
        ?? @"Server=(localdb)\MSSQLLocalDB;Database=QuanLyNguoiDung;Trusted_Connection=True;TrustServerCertificate=True;";

    public static async Task<bool> KiemTraTenDangNhapTonTai(string tenDangNhap)
    {
        const string sql = "SELECT COUNT(*) FROM Users WHERE TenDangNhap = @ten";
        await using var conn = new SqlConnection(ChuoiKetNoi);
        await conn.OpenAsync();

        await using var cmd = new SqlCommand(sql, conn);
        cmd.Parameters.Add("@ten", SqlDbType.NVarChar, 20).Value = tenDangNhap;
        return Convert.ToInt32(await cmd.ExecuteScalarAsync()) > 0;
    }

    public static async Task DangKy(string tenDangNhap, string matKhau, string hoTen, string email)
    {
        (string salt, string bam) = MatKhau.Tao(matKhau);
        const string sql = @"INSERT INTO Users (TenDangNhap, MatKhauBam, Salt, HoTen, Email)
                             VALUES (@ten, @bam, @salt, @hoTen, @email)";

        await using var conn = new SqlConnection(ChuoiKetNoi);
        await conn.OpenAsync();

        await using var cmd = new SqlCommand(sql, conn);
        cmd.Parameters.Add("@ten", SqlDbType.NVarChar, 20).Value = tenDangNhap;
        cmd.Parameters.Add("@bam", SqlDbType.NVarChar, 64).Value = bam;
        cmd.Parameters.Add("@salt", SqlDbType.NVarChar, 32).Value = salt;
        cmd.Parameters.Add("@hoTen", SqlDbType.NVarChar, 50).Value = hoTen;
        cmd.Parameters.Add("@email", SqlDbType.NVarChar, 100).Value = email;
        await cmd.ExecuteNonQueryAsync();
    }

    public static async Task<NguoiDung?> DangNhap(string tenDangNhap, string matKhau)
    {
        const string sql = @"SELECT MatKhauBam, Salt, HoTen, Email, NgayTao, LanDangNhapCuoi
                             FROM Users WHERE TenDangNhap = @ten";
        NguoiDung? nguoiDung = null;

        await using var conn = new SqlConnection(ChuoiKetNoi);
        await conn.OpenAsync();

        await using (var cmd = new SqlCommand(sql, conn))
        {
            cmd.Parameters.Add("@ten", SqlDbType.NVarChar, 20).Value = tenDangNhap;
            await using var reader = await cmd.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                string bam = reader.GetString(0);
                string salt = reader.GetString(1);

                if (MatKhau.KiemTra(matKhau, salt, bam))
                {
                    nguoiDung = new NguoiDung(
                        tenDangNhap,
                        reader.GetString(2),
                        reader.IsDBNull(3) ? "" : reader.GetString(3),
                        reader.GetDateTime(4),
                        reader.IsDBNull(5) ? null : reader.GetDateTime(5));
                }
            }
        }

        if (nguoiDung is null)
        {
            return null;
        }

        const string sqlCapNhat = "UPDATE Users SET LanDangNhapCuoi = SYSDATETIME() WHERE TenDangNhap = @ten";
        await using (var cmd = new SqlCommand(sqlCapNhat, conn))
        {
            cmd.Parameters.Add("@ten", SqlDbType.NVarChar, 20).Value = tenDangNhap;
            await cmd.ExecuteNonQueryAsync();
        }

        return nguoiDung;
    }

    public static async Task<NguoiDung?> LayThongTin(string tenDangNhap)
    {
        const string sql = @"SELECT HoTen, Email, NgayTao, LanDangNhapCuoi
                             FROM Users WHERE TenDangNhap = @ten";

        await using var conn = new SqlConnection(ChuoiKetNoi);
        await conn.OpenAsync();

        await using var cmd = new SqlCommand(sql, conn);
        cmd.Parameters.Add("@ten", SqlDbType.NVarChar, 20).Value = tenDangNhap;
        await using var reader = await cmd.ExecuteReaderAsync();

        if (!await reader.ReadAsync())
        {
            return null;
        }

        return new NguoiDung(
            tenDangNhap,
            reader.GetString(0),
            reader.IsDBNull(1) ? "" : reader.GetString(1),
            reader.GetDateTime(2),
            reader.IsDBNull(3) ? null : reader.GetDateTime(3));
    }
}
