using System.Buffers.Binary;

namespace BaiTap3.Chung;

/// <summary>
/// Đóng khung thông điệp trên TCP: [4 byte độ dài, big-endian][nội dung].
/// TCP là luồng byte không có ranh giới thông điệp: hai lần Write có thể bị gộp thành một lần Read,
/// hoặc một lần Write bị cắt thành nhiều lần Read. Tiền tố độ dài cho biết chính xác phải đọc bao nhiêu byte.
/// </summary>
public static class KhungThongDiep
{
    public const int KichThuocToiDa = 64 * 1024;

    public static async Task GhiAsync(Stream luong, byte[] noiDung, CancellationToken token = default)
    {
        if (noiDung.Length > KichThuocToiDa)
            throw new InvalidDataException("Thông điệp vượt quá kích thước cho phép.");

        var tieuDe = new byte[4];
        BinaryPrimitives.WriteInt32BigEndian(tieuDe, noiDung.Length);

        await luong.WriteAsync(tieuDe, token);
        await luong.WriteAsync(noiDung, token);
        await luong.FlushAsync(token);
    }

    /// <summary>Đọc một thông điệp. Trả về null nếu đối phương đã đóng kết nối đúng ranh giới thông điệp.</summary>
    /// <exception cref="InvalidDataException">Độ dài khai báo không hợp lệ.</exception>
    /// <exception cref="EndOfStreamException">Kết nối đứt giữa chừng một thông điệp.</exception>
    public static async Task<byte[]?> DocAsync(Stream luong, CancellationToken token = default)
    {
        var tieuDe = new byte[4];
        if (!await DocDuAsync(luong, tieuDe, token))
            return null;

        var doDai = BinaryPrimitives.ReadInt32BigEndian(tieuDe);
        if (doDai < 0 || doDai > KichThuocToiDa)
            throw new InvalidDataException("Độ dài thông điệp không hợp lệ.");

        var noiDung = new byte[doDai];
        if (!await DocDuAsync(luong, noiDung, token))
            throw new EndOfStreamException();
        return noiDung;
    }

    // Đọc cho đủ đúng buffer.Length byte. false = kết nối đóng trước khi nhận byte nào.
    private static async Task<bool> DocDuAsync(Stream luong, byte[] buffer, CancellationToken token)
    {
        var daDoc = 0;
        while (daDoc < buffer.Length)
        {
            var n = await luong.ReadAsync(buffer.AsMemory(daDoc), token);
            if (n == 0)
            {
                if (daDoc == 0) return false;
                throw new EndOfStreamException();
            }
            daDoc += n;
        }
        return true;
    }
}
