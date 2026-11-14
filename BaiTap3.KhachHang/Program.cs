namespace BaiTap3.KhachHang;

internal static class Program
{
    [STAThread]
    static void Main()
    {
        ApplicationConfiguration.Initialize();
        Application.Run(new DangKy());
    }
}
