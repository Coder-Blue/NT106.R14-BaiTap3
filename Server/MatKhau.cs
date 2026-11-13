using System.Security.Cryptography;

namespace CRUD_APP;

public static class MatKhau
{
    // PBKDF2-HMAC-SHA256 với 600.000 vòng lặp theo khuyến nghị OWASP.
    private const int VongLap = 600_000;
    private const int KichThuocSalt = 16;
    private const int KichThuocBam = 32;

    public static (string Salt, string Bam) Tao(string matKhau)
    {
        var salt = RandomNumberGenerator.GetBytes(KichThuocSalt);
        var bam = Rfc2898DeriveBytes.Pbkdf2(matKhau, salt, VongLap, HashAlgorithmName.SHA256, KichThuocBam);
        return (Convert.ToBase64String(salt), Convert.ToBase64String(bam));
    }

    public static bool KiemTra(string matKhau, string saltLuu, string bamLuu)
    {
        try
        {
            var salt = Convert.FromBase64String(saltLuu);
            var bamCu = Convert.FromBase64String(bamLuu);
            var bamMoi = Rfc2898DeriveBytes.Pbkdf2(matKhau, salt, VongLap, HashAlgorithmName.SHA256, KichThuocBam);

            // So sánh an toàn theo thời gian để không lộ thông tin qua tốc độ thực thi.
            return CryptographicOperations.FixedTimeEquals(bamCu, bamMoi);
        }
        catch (FormatException)
        {
            return false;
        }
    }
}
