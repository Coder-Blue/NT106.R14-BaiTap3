namespace NT106.R14.BaiTap3.Server;

internal static class Program
{
    [STAThread]
    private static void Main()
    {
        ApplicationConfiguration.Initialize();
        Application.Run(new FormServer());
    }
}
