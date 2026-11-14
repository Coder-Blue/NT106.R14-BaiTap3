using System.Diagnostics;
using System.Net.Sockets;
using NT106.R14.BaiTap3.Shared;

namespace NT106.R14.BaiTap3.Client.Forms;

public sealed class MenuChinh : Form
{
    private readonly Label lblTrangThai;

    public MenuChinh()
    {
        Text = "Bài tập 3 - Chọn chế độ";
        ClientSize = new Size(520, 420);
        BackColor = Color.White;
        Font = new Font("Segoe UI", 9F);
        FormBorderStyle = FormBorderStyle.FixedSingle;
        MaximizeBox = false;
        MinimizeBox = false;
        StartPosition = FormStartPosition.CenterScreen;

        var lblTieuDe = new Label
        {
            Name = "lblTieuDe",
            Text = "CHỌN CHẾ ĐỘ",
            Font = new Font("Segoe UI", 22F, FontStyle.Bold),
            ForeColor = Color.FromArgb(35, 78, 120),
            TextAlign = ContentAlignment.MiddleCenter,
            Location = new Point(0, 32),
            Size = new Size(520, 44),
        };

        var lblPhuDe = new Label
        {
            Name = "lblPhuDe",
            Text = "Mở Client để đăng nhập hoặc mở Server để lắng nghe kết nối",
            Font = new Font("Segoe UI", 10F),
            ForeColor = Color.FromArgb(100, 100, 100),
            TextAlign = ContentAlignment.MiddleCenter,
            Location = new Point(0, 80),
            Size = new Size(520, 24),
        };

        Button btnClient = TaoNut("Mở Client", "btnClient", 140, Color.FromArgb(51, 122, 183));
        btnClient.Click += btnClient_Click;

        Button btnServer = TaoNut("Mở Server", "btnServer", 210, Color.FromArgb(40, 140, 70));
        btnServer.Click += btnServer_Click;

        var btnThoat = new Button
        {
            Name = "btnThoat",
            Text = "Thoát",
            Font = new Font("Segoe UI", 10.5F),
            ForeColor = Color.FromArgb(64, 64, 64),
            BackColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Size = new Size(240, 46),
            Location = new Point(140, 280),
            Cursor = Cursors.Hand,
        };
        btnThoat.FlatAppearance.BorderColor = Color.FromArgb(200, 200, 200);
        btnThoat.Click += (_, _) => Close();

        lblTrangThai = new Label
        {
            Name = "lblTrangThai",
            Text = "Chọn một chế độ để bắt đầu.",
            Font = new Font("Segoe UI", 9F, FontStyle.Italic),
            ForeColor = Color.FromArgb(100, 100, 100),
            TextAlign = ContentAlignment.MiddleCenter,
            Location = new Point(0, 350),
            Size = new Size(520, 24),
        };

        Controls.Add(lblTieuDe);
        Controls.Add(lblPhuDe);
        Controls.Add(btnClient);
        Controls.Add(btnServer);
        Controls.Add(btnThoat);
        Controls.Add(lblTrangThai);
    }

    private static Button TaoNut(string text, string name, int y, Color mau)
    {
        var nut = new Button
        {
            Name = name,
            Text = text,
            Font = new Font("Segoe UI", 10.5F, FontStyle.Bold),
            ForeColor = Color.White,
            BackColor = mau,
            FlatStyle = FlatStyle.Flat,
            Size = new Size(240, 46),
            Location = new Point(140, y),
            Cursor = Cursors.Hand,
        };
        nut.FlatAppearance.BorderSize = 0;
        return nut;
    }

    private void btnClient_Click(object? sender, EventArgs e)
    {
        lblTrangThai.Text = "Đang mở Client...";
        using var form = new DangNhap();
        form.ShowDialog(this);
        lblTrangThai.Text = "Chọn một chế độ để bắt đầu.";
    }

    private async void btnServer_Click(object? sender, EventArgs e)
    {
        Button? nut = sender as Button;
        if (nut is not null)
        {
            nut.Enabled = false;
        }

        try
        {
            if (await CongDangMoAsync())
            {
                lblTrangThai.Text = $"Server đang chạy sẵn tại cổng {GiaoThuc.CongMacDinh}.";
                return;
            }

            string? duongDan = TimServer();
            if (duongDan is null)
            {
                MessageBox.Show(this,
                    "Chưa tìm thấy Server đã build. Hãy build solution (Ctrl+Shift+B) rồi thử lại.",
                    "Chưa có Server", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            Process.Start(new ProcessStartInfo
            {
                FileName = duongDan,
                WorkingDirectory = Path.GetDirectoryName(duongDan)!,
                UseShellExecute = true,
            });
            lblTrangThai.Text = "Đã mở Server trong cửa sổ console.";
        }
        catch (Exception ex) when (ex is IOException or InvalidOperationException or System.ComponentModel.Win32Exception)
        {
            MessageBox.Show(this, "Không mở được Server: " + ex.Message,
                "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        finally
        {
            if (nut is not null)
            {
                nut.Enabled = true;
            }
        }
    }

    private static async Task<bool> CongDangMoAsync()
    {
        try
        {
            using var client = new TcpClient();
            await client.ConnectAsync("127.0.0.1", GiaoThuc.CongMacDinh)
                .WaitAsync(TimeSpan.FromMilliseconds(500));
            return true;
        }
        catch
        {
            return false;
        }
    }

    private static string? TimServer()
    {
        string? goc = TimThuMucGoc();
        if (goc is null)
        {
            return null;
        }

        string thuMucBin = Path.Combine(goc, "Server", "bin");
        if (!Directory.Exists(thuMucBin))
        {
            return null;
        }

        return Directory.GetFiles(thuMucBin, "NT106.R14-BaiTap3.Server.exe", SearchOption.AllDirectories)
            .OrderByDescending(File.GetLastWriteTime)
            .FirstOrDefault();
    }

    private static string? TimThuMucGoc()
    {
        foreach (string noiBatDau in new[] { AppContext.BaseDirectory, Directory.GetCurrentDirectory() })
        {
            DirectoryInfo? thuMuc = new(noiBatDau);
            while (thuMuc is not null)
            {
                if (File.Exists(Path.Combine(thuMuc.FullName, "NT106.R14-BaiTap3.slnx")))
                {
                    return thuMuc.FullName;
                }

                thuMuc = thuMuc.Parent;
            }
        }

        return null;
    }
}
