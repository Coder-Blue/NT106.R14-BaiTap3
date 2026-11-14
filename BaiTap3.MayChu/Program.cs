using BaiTap3.Chung;
using BaiTap3.MayChu;
using Microsoft.Data.SqlClient;

Console.OutputEncoding = System.Text.Encoding.UTF8; // sửa lỗi dấu ? trong log

var cong = args.Length > 0 && int.TryParse(args[0], out var c) ? c : GiaoThuc.CongMacDinh;

using var cts = new CancellationTokenSource();
Console.CancelKeyPress += (_, e) =>
{
    e.Cancel = true; // để finally trong ChayAsync kịp chạy
    NhatKy.Ghi("Nhận Ctrl+C, đang dừng máy chủ...");
    cts.Cancel();
};

try
{
    await XuLyNguoiDung.KhoiTaoAsync(NhatKy.Ghi, cts.Token);
}
catch (SqlException ex)
{
    NhatKy.Ghi($"KHÔNG khởi tạo được CSDL (mã {ex.Number}): {ex.Message}");
    NhatKy.Ghi("Kiểm tra chuỗi kết nối ở đầu XuLyNguoiDung.cs. Server vẫn chạy nhưng đăng ký sẽ báo lỗi CSDL.");
}

await new MayChuTcp().ChayAsync(cong, cts.Token);