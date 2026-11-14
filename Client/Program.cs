using NT106.R14.BaiTap3.Client.Forms;

namespace NT106.R14.BaiTap3.Client;

internal static class Program
{
    [STAThread]
    private static void Main()
    {
        ApplicationConfiguration.Initialize();
        Application.Run(new MenuChinh());
    }
}
