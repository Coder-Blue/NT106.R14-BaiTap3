using System.Security.Cryptography;

namespace NT106.R14.BaiTap3.Server;

public static class MatKhau
{
    private const int VongLap = 600_000;
    private const int KichThuocSalt = 16;
    private const int KichThuocBam = 32;
    private const int KichThuocToken = 32;

    public static (string Salt, string Bam) Tao(string matKhau)
    {
        byte[] salt = RandomNumberGenerator.GetBytes(KichThuocSalt);
        byte[] bam = Rfc2898DeriveBytes.Pbkdf2(matKhau, salt, VongLap, HashAlgorithmName.SHA256, KichThuocBam);
        return (Convert.ToBase64String(salt), Convert.ToBase64String(bam));
    }

    public static bool KiemTra(string matKhau, string saltLuu, string bamLuu)
    {
        try
        {
            byte[] salt = Convert.FromBase64String(saltLuu);
            byte[] bamCu = Convert.FromBase64String(bamLuu);
            byte[] bamMoi = Rfc2898DeriveBytes.Pbkdf2(matKhau, salt, VongLap, HashAlgorithmName.SHA256, KichThuocBam);
            return CryptographicOperations.FixedTimeEquals(bamCu, bamMoi);
        }
        catch (FormatException)
        {
            return false;
        }
    }

    public static string TaoToken() =>
        Convert.ToHexString(RandomNumberGenerator.GetBytes(KichThuocToken));
}
