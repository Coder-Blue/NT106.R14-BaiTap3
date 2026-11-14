using System.Data;
using Microsoft.Data.SqlClient;

namespace BaiTap3.MayChu;

public enum KetQuaDangKy { ThanhCong, TenDaTonTai }

public static class XuLyNguoiDung
{
    // Đổi instance cho đúng với máy cài SQL Server nếu cần.
    private const string ChuoiKetNoi =
        @"Server=(localdb)\MSSQLLocalDB;Database=QuanLyNguoiDung;Trusted_Connection=True;TrustServerCertificate=True;";

    // Tên bảng và cột theo đề bài (mục III.6).
    private const string Bang = "NguoiDung";
    private const string CotTen = "Username";
    private const string CotBam = "PasswordHash";
    private const string CotSalt = "PasswordSalt";
    private const string CotHoTen = "FullName";
    private const string CotEmail = "Email";

    /// <summary>Chạy khi server khởi động: tạo CSDL và bảng nếu thiếu, ghi log thông tin kết nối.</summary>
    /// <exception cref="SqlException">Không kết nối hoặc không tạo được CSDL.</exception>
    public static async Task KhoiTaoAsync(Action<string> ghiNhatKy, CancellationToken token)
    {
        var builder = new SqlConnectionStringBuilder(ChuoiKetNoi);
        var tenCsdl = builder.InitialCatalog;

        // Bước 1: tạo CSDL nếu chưa có (kết nối vào master).
        builder.InitialCatalog = "master";
        await using (var connMaster = new SqlConnection(builder.ConnectionString))
        {
            await connMaster.OpenAsync(token);
            const string sqlTaoCsdl = @"IF DB_ID(@ten) IS NULL
                BEGIN
                    DECLARE @sql NVARCHAR(300) = N'CREATE DATABASE ' + QUOTENAME(@ten);
                    EXEC(@sql);
                END";
            await using var cmd = new SqlCommand(sqlTaoCsdl, connMaster);
            cmd.Parameters.Add("@ten", SqlDbType.NVarChar, 128).Value = tenCsdl;
            await cmd.ExecuteNonQueryAsync(token);
        }

        await using var conn = new SqlConnection(ChuoiKetNoi);
        await conn.OpenAsync(token);

        // Bước 2: ghi log đang nối vào đâu (để đối chiếu với SSMS).
        await using (var cmd = new SqlCommand("SELECT @@SERVERNAME, DB_NAME()", conn))
        await using (var reader = await cmd.ExecuteReaderAsync(token))
        {
            if (await reader.ReadAsync(token))
                ghiNhatKy($"Đã kết nối SQL Server '{reader.GetString(0)}', CSDL '{reader.GetString(1)}'.");
        }

        // Bước 3: kiểm tra bảng và các cột cần dùng.
        bool bangTonTai;
        await using (var cmd = new SqlCommand(
            "SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = @bang", conn))
        {
            cmd.Parameters.Add("@bang", SqlDbType.NVarChar, 128).Value = Bang;
            bangTonTai = Convert.ToInt32(await cmd.ExecuteScalarAsync(token)) > 0;
        }

        if (bangTonTai)
        {
            int soCotDung;
            const string sqlDemCot = @"SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS
                WHERE TABLE_NAME = @bang AND COLUMN_NAME IN (@c1, @c2, @c3, @c4, @c5)";
            await using (var cmd = new SqlCommand(sqlDemCot, conn))
            {
                cmd.Parameters.Add("@bang", SqlDbType.NVarChar, 128).Value = Bang;
                cmd.Parameters.Add("@c1", SqlDbType.NVarChar, 128).Value = CotTen;
                cmd.Parameters.Add("@c2", SqlDbType.NVarChar, 128).Value = CotBam;
                cmd.Parameters.Add("@c3", SqlDbType.NVarChar, 128).Value = CotSalt;
                cmd.Parameters.Add("@c4", SqlDbType.NVarChar, 128).Value = CotHoTen;
                cmd.Parameters.Add("@c5", SqlDbType.NVarChar, 128).Value = CotEmail;
                soCotDung = Convert.ToInt32(await cmd.ExecuteScalarAsync(token));
            }

            if (soCotDung == 5)
            {
                ghiNhatKy($"Bảng {Bang} đã có đủ cột cần dùng.");
                return;
            }

            // Bảng cũ thiếu cột: đổi tên để giữ lại dữ liệu, không xóa.
            var tenCu = $"{Bang}_Cu_{DateTime.Now:yyyyMMddHHmmss}";
            await using var cmdDoiTen = new SqlCommand("sp_rename", conn) { CommandType = CommandType.StoredProcedure };
            cmdDoiTen.Parameters.Add("@objname", SqlDbType.NVarChar, 776).Value = $"dbo.{Bang}";
            cmdDoiTen.Parameters.Add("@newname", SqlDbType.NVarChar, 128).Value = tenCu;
            await cmdDoiTen.ExecuteNonQueryAsync(token);
            ghiNhatKy($"Bảng {Bang} cũ thiếu cột, đã đổi tên thành {tenCu}.");
        }

        const string sqlTaoBang = $@"CREATE TABLE dbo.{Bang} (
            UserId        INT IDENTITY(1,1) PRIMARY KEY,
            {CotTen}      NVARCHAR(20)  NOT NULL UNIQUE,
            {CotBam}      NVARCHAR(64)  NOT NULL,
            {CotSalt}     NVARCHAR(32)  NOT NULL,
            {CotEmail}    NVARCHAR(100) NULL,
            {CotHoTen}    NVARCHAR(50)  NOT NULL,
            Birthday      DATE          NULL)";
        await using (var cmd = new SqlCommand(sqlTaoBang, conn))
        {
            await cmd.ExecuteNonQueryAsync(token);
        }
        ghiNhatKy($"Đã tạo bảng {Bang}.");
    }

    /// <exception cref="SqlException">Không truy cập được cơ sở dữ liệu.</exception>
    public static async Task<KetQuaDangKy> DangKyAsync(
        string tenDangNhap, string matKhau, string hoTen, string email, CancellationToken token)
    {
        await using var conn = new SqlConnection(ChuoiKetNoi);
        await conn.OpenAsync(token);

        if (await TenDangNhapTonTaiAsync(conn, tenDangNhap, token))
            return KetQuaDangKy.TenDaTonTai;

        // Băm ở phía server, ngay trước khi ghi. PBKDF2 tốn CPU nên chạy ngoài luồng đang xử lý.
        var (salt, bam) = await Task.Run(() => MatKhau.Tao(matKhau), token);

        const string sql = $@"INSERT INTO {Bang} ({CotTen}, {CotBam}, {CotSalt}, {CotHoTen}, {CotEmail})
                              VALUES (@ten, @bam, @salt, @hoTen, @email)";
        try
        {
            await using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.Add("@ten", SqlDbType.NVarChar, 20).Value = tenDangNhap;
            cmd.Parameters.Add("@bam", SqlDbType.NVarChar, 64).Value = bam;
            cmd.Parameters.Add("@salt", SqlDbType.NVarChar, 32).Value = salt;
            cmd.Parameters.Add("@hoTen", SqlDbType.NVarChar, 50).Value = hoTen;
            cmd.Parameters.Add("@email", SqlDbType.NVarChar, 100).Value = email.Length == 0 ? DBNull.Value : email;
            await cmd.ExecuteNonQueryAsync(token);
            return KetQuaDangKy.ThanhCong;
        }
        catch (SqlException ex) when (ex.Number is 2627 or 2601)
        {
            // Hai client cùng đăng ký một tên cùng lúc: ràng buộc UNIQUE là chốt chặn cuối.
            return KetQuaDangKy.TenDaTonTai;
        }
    }

    private static async Task<bool> TenDangNhapTonTaiAsync(SqlConnection conn, string tenDangNhap, CancellationToken token)
    {
        const string sql = $"SELECT COUNT(*) FROM {Bang} WHERE {CotTen} = @ten";
        await using var cmd = new SqlCommand(sql, conn);
        cmd.Parameters.Add("@ten", SqlDbType.NVarChar, 20).Value = tenDangNhap;
        return Convert.ToInt32(await cmd.ExecuteScalarAsync(token)) > 0;
    }
}