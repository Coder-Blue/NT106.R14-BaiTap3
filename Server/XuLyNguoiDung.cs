using System.Data;
using Microsoft.Data.SqlClient;
using NT106.R14.BaiTap3.Shared;

namespace NT106.R14.BaiTap3.Server;

public static class XuLyNguoiDung
{
    private static readonly string ChuoiKetNoi = CauHinh.ChuoiKetNoi;

    public static async Task KhoiTaoAsync()
    {
        var builder = new SqlConnectionStringBuilder(ChuoiKetNoi);
        string tenCsdl = builder.InitialCatalog;
        if (tenCsdl.Length == 0)
        {
            throw new InvalidOperationException("Chuỗi kết nối phải có tên cơ sở dữ liệu.");
        }

        builder.InitialCatalog = "master";
        await using (var conn = new SqlConnection(builder.ConnectionString))
        {
            await conn.OpenAsync();
            const string sql = @"
IF DB_ID(@ten) IS NULL
BEGIN
    DECLARE @lenh NVARCHAR(300) = N'CREATE DATABASE ' + QUOTENAME(@ten);
    EXEC(@lenh);
END";
            await using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.Add("@ten", SqlDbType.NVarChar, 128).Value = tenCsdl;
            await cmd.ExecuteNonQueryAsync();
        }

        await using (var conn = new SqlConnection(ChuoiKetNoi))
        {
            await conn.OpenAsync();
            const string sql = @"
IF OBJECT_ID(N'dbo.NguoiDung', N'U') IS NULL
CREATE TABLE dbo.NguoiDung (
    UserId       INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_NguoiDung PRIMARY KEY,
    Username     NVARCHAR(20)  NOT NULL CONSTRAINT UQ_NguoiDung_Username UNIQUE,
    PasswordHash NVARCHAR(64)  NOT NULL,
    PasswordSalt NVARCHAR(32)  NOT NULL,
    Email        NVARCHAR(100) NOT NULL,
    FullName     NVARCHAR(50)  NOT NULL,
    Birthday     DATE          NOT NULL
);";
            await using var cmd = new SqlCommand(sql, conn);
            await cmd.ExecuteNonQueryAsync();
        }
    }

    public static async Task<bool> KiemTraTenDangNhapTonTai(string tenDangNhap)
    {
        const string sql = "SELECT COUNT(*) FROM NguoiDung WHERE Username = @ten";
        await using var conn = new SqlConnection(ChuoiKetNoi);
        await conn.OpenAsync();

        await using var cmd = new SqlCommand(sql, conn);
        cmd.Parameters.Add("@ten", SqlDbType.NVarChar, 20).Value = tenDangNhap;
        return Convert.ToInt32(await cmd.ExecuteScalarAsync()) > 0;
    }

    public static async Task DangKy(string tenDangNhap, string matKhau, string hoTen, string email, DateTime ngaySinh)
    {
        (string salt, string bam) = await Task.Run(() => MatKhau.Tao(matKhau));
        const string sql = @"INSERT INTO NguoiDung (Username, PasswordHash, PasswordSalt, FullName, Email, Birthday)
                             VALUES (@ten, @bam, @salt, @hoTen, @email, @ngaySinh)";

        await using var conn = new SqlConnection(ChuoiKetNoi);
        await conn.OpenAsync();

        await using var cmd = new SqlCommand(sql, conn);
        cmd.Parameters.Add("@ten", SqlDbType.NVarChar, 20).Value = tenDangNhap;
        cmd.Parameters.Add("@bam", SqlDbType.NVarChar, 64).Value = bam;
        cmd.Parameters.Add("@salt", SqlDbType.NVarChar, 32).Value = salt;
        cmd.Parameters.Add("@hoTen", SqlDbType.NVarChar, 50).Value = hoTen;
        cmd.Parameters.Add("@email", SqlDbType.NVarChar, 100).Value = email;
        cmd.Parameters.Add("@ngaySinh", SqlDbType.Date).Value = ngaySinh;
        await cmd.ExecuteNonQueryAsync();
    }

    public static async Task<NguoiDung?> DangNhap(string tenDangNhap, string matKhau)
    {
        const string sql = @"SELECT UserId, PasswordHash, PasswordSalt, FullName, Email, Birthday
                             FROM NguoiDung WHERE Username = @ten";
        NguoiDung? nguoiDung = null;

        await using var conn = new SqlConnection(ChuoiKetNoi);
        await conn.OpenAsync();

        await using var cmd = new SqlCommand(sql, conn);
        cmd.Parameters.Add("@ten", SqlDbType.NVarChar, 20).Value = tenDangNhap;
        await using var reader = await cmd.ExecuteReaderAsync();

        if (await reader.ReadAsync())
        {
            string bam = reader.GetString(1);
            string salt = reader.GetString(2);

            if (MatKhau.KiemTra(matKhau, salt, bam))
            {
                nguoiDung = new NguoiDung(
                    reader.GetInt32(0),
                    tenDangNhap,
                    reader.GetString(3),
                    reader.GetString(4),
                    reader.GetDateTime(5));
            }
        }

        return nguoiDung;
    }

    public static async Task<NguoiDung?> LayThongTin(string tenDangNhap)
    {
        const string sql = @"SELECT UserId, FullName, Email, Birthday
                             FROM NguoiDung WHERE Username = @ten";

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
            reader.GetInt32(0),
            tenDangNhap,
            reader.GetString(1),
            reader.GetString(2),
            reader.GetDateTime(3));
    }
}
