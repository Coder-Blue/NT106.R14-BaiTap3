using System.Text.Json;
using System.Text.Json.Serialization;

namespace BaiTap3.Chung;

/// <summary>Yêu cầu client gửi lên server.</summary>
public sealed record YeuCau(string Lenh, Dictionary<string, string>? ThamSo = null)
{
    public string LayThamSo(string khoa) =>
        ThamSo is not null && ThamSo.TryGetValue(khoa, out var giaTri) ? giaTri : "";
}

/// <summary>Phản hồi server trả về. Ma = 0 là thành công.</summary>
public sealed record PhanHoi(int Ma, string ThongBao, Dictionary<string, string>? DuLieu = null)
{
    [JsonIgnore]
    public bool ThanhCong => Ma == MaLoi.ThanhCong;

    public static PhanHoi TaoThanhCong(string thongBao, Dictionary<string, string>? duLieu = null) =>
        new(MaLoi.ThanhCong, thongBao, duLieu);

    public static PhanHoi TaoLoi(int ma, string thongBao) => new(ma, thongBao);
}

/// <summary>Chuyển thông điệp thành/từ mảng byte UTF-8 (JSON).</summary>
public static class BoMaHoa
{
    private static readonly JsonSerializerOptions TuyChon = new(JsonSerializerDefaults.Web);

    public static byte[] DongGoi<T>(T doiTuong) => JsonSerializer.SerializeToUtf8Bytes(doiTuong, TuyChon);

    /// <exception cref="JsonException">Dữ liệu không phải JSON hợp lệ.</exception>
    public static T? MoGoi<T>(byte[] duLieu) => JsonSerializer.Deserialize<T>(duLieu, TuyChon);
}
