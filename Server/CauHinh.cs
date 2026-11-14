using System.Text.Json;

namespace NT106.R14.BaiTap3.Server;

public static class CauHinh
{
    private const string ChuoiKetNoiMacDinh =
        @"Server=(localdb)\MSSQLLocalDB;Database=QuanLyNguoiDung;Trusted_Connection=True;TrustServerCertificate=True;";

    public static readonly string ChuoiKetNoi = DocChuoiKetNoi();

    private static string DocChuoiKetNoi()
    {
        string? tuMoiTruong = Environment.GetEnvironmentVariable("BAITAP3_CONNECTION");
        if (!string.IsNullOrWhiteSpace(tuMoiTruong))
        {
            return tuMoiTruong;
        }

        string duongDan = Path.Combine(AppContext.BaseDirectory, "appsettings.json");
        if (!File.Exists(duongDan))
        {
            return ChuoiKetNoiMacDinh;
        }

        try
        {
            string json = File.ReadAllText(duongDan);
            using JsonDocument taiLieu = JsonDocument.Parse(json);
            if (taiLieu.RootElement.TryGetProperty("ChuoiKetNoi", out JsonElement phanTu) &&
                phanTu.ValueKind == JsonValueKind.String)
            {
                string? chuoi = phanTu.GetString();
                if (!string.IsNullOrWhiteSpace(chuoi))
                {
                    return chuoi;
                }
            }
        }
        catch (JsonException)
        {
        }
        catch (IOException)
        {
        }

        return ChuoiKetNoiMacDinh;
    }
}
