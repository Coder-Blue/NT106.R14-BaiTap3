using NT106.R14.BaiTap3.Shared;

namespace NT106.R14.BaiTap3.Server;

public sealed class FormServer : Form
{
    private readonly MayChu _mayChu = new();
    private readonly System.Windows.Forms.Timer _timerCapNhat = new() { Interval = 1000 };

    private readonly Label _lblTrangThai = new();
    private readonly Button _btnBatDau = new();
    private readonly Button _btnDung = new();
    private readonly ListView _lvClient = new();
    private readonly TextBox _txtLog = new();

    public FormServer()
    {
        Text = "Server - Bài tập 3";
        ClientSize = new Size(860, 620);
        BackColor = Color.White;
        Font = new Font("Segoe UI", 9F);
        FormBorderStyle = FormBorderStyle.FixedSingle;
        MaximizeBox = false;
        MinimizeBox = false;
        StartPosition = FormStartPosition.CenterScreen;

        TaoGiaoDien();

        _mayChu.CoLog += MayChu_CoLog;
        _timerCapNhat.Tick += (_, _) => CapNhatGiaoDien();
        _timerCapNhat.Start();
        Shown += async (_, _) => await BatDauAsync();
        FormClosing += FormServer_FormClosing;
    }

    private void TaoGiaoDien()
    {
        var lblTieuDe = new Label
        {
            Text = "SERVER - BÀI TẬP 3",
            Font = new Font("Segoe UI", 20F, FontStyle.Bold),
            ForeColor = Color.FromArgb(35, 78, 120),
            TextAlign = ContentAlignment.MiddleCenter,
            Location = new Point(0, 18),
            Size = new Size(860, 40),
        };

        _lblTrangThai.Name = "lblTrangThai";
        _lblTrangThai.Font = new Font("Segoe UI", 10F, FontStyle.Italic);
        _lblTrangThai.ForeColor = Color.FromArgb(90, 90, 90);
        _lblTrangThai.Location = new Point(30, 70);
        _lblTrangThai.Size = new Size(560, 24);
        _lblTrangThai.Text = "Trạng thái: Đã dừng";

        TaoNut(_btnBatDau, "Bắt đầu", 640, Color.FromArgb(40, 140, 70));
        _btnBatDau.Name = "btnBatDau";
        _btnBatDau.Click += btnBatDau_Click;

        TaoNut(_btnDung, "Dừng", 745, Color.FromArgb(200, 70, 70));
        _btnDung.Name = "btnDung";
        _btnDung.Click += btnDung_Click;

        var lblClient = new Label
        {
            Text = "Client đang kết nối",
            Font = new Font("Segoe UI", 10F, FontStyle.Bold),
            ForeColor = Color.FromArgb(50, 50, 50),
            Location = new Point(30, 105),
            Size = new Size(300, 22),
        };

        _lvClient.Name = "lvClient";
        _lvClient.View = View.Details;
        _lvClient.FullRowSelect = true;
        _lvClient.GridLines = true;
        _lvClient.HideSelection = false;
        _lvClient.Location = new Point(30, 130);
        _lvClient.Size = new Size(800, 180);
        _lvClient.Columns.Add("Địa chỉ", 220);
        _lvClient.Columns.Add("Tài khoản", 220);
        _lvClient.Columns.Add("Hết hạn", 350);

        var lblLog = new Label
        {
            Text = "Nhật ký server",
            Font = new Font("Segoe UI", 10F, FontStyle.Bold),
            ForeColor = Color.FromArgb(50, 50, 50),
            Location = new Point(30, 325),
            Size = new Size(300, 22),
        };

        _txtLog.Name = "txtLog";
        _txtLog.Multiline = true;
        _txtLog.ReadOnly = true;
        _txtLog.ScrollBars = ScrollBars.Both;
        _txtLog.WordWrap = false;
        _txtLog.Font = new Font("Consolas", 9F);
        _txtLog.BackColor = Color.FromArgb(245, 245, 245);
        _txtLog.Location = new Point(30, 350);
        _txtLog.Size = new Size(800, 235);

        var lblChan = new Label
        {
            Text = $"TCP {GiaoThuc.CongMacDinh} • UDP {GiaoThuc.CongUdpDiscovery}",
            ForeColor = Color.FromArgb(120, 120, 120),
            TextAlign = ContentAlignment.MiddleRight,
            Location = new Point(600, 590),
            Size = new Size(230, 20),
        };

        Controls.Add(lblTieuDe);
        Controls.Add(_lblTrangThai);
        Controls.Add(_btnBatDau);
        Controls.Add(_btnDung);
        Controls.Add(lblClient);
        Controls.Add(_lvClient);
        Controls.Add(lblLog);
        Controls.Add(_txtLog);
        Controls.Add(lblChan);
    }

    private static void TaoNut(Button nut, string text, int x, Color mau)
    {
        nut.Text = text;
        nut.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
        nut.ForeColor = Color.White;
        nut.BackColor = mau;
        nut.FlatStyle = FlatStyle.Flat;
        nut.FlatAppearance.BorderSize = 0;
        nut.Location = new Point(x, 65);
        nut.Size = new Size(95, 32);
        nut.Cursor = Cursors.Hand;
    }

    private async Task BatDauAsync()
    {
        _btnBatDau.Enabled = false;
        _btnDung.Enabled = false;
        try
        {
            await _mayChu.BatDauAsync();
        }
        // Không để lộ chi tiết ngoại lệ ra giao diện.
        catch (Exception)
        {
        }

        CapNhatGiaoDien();
    }

    private async void btnBatDau_Click(object? sender, EventArgs e)
    {
        await BatDauAsync();
    }

    private async void btnDung_Click(object? sender, EventArgs e)
    {
        _btnBatDau.Enabled = false;
        _btnDung.Enabled = false;
        try
        {
            await _mayChu.DungAsync();
        }
        // Không để lộ chi tiết ngoại lệ ra giao diện.
        catch (Exception)
        {
        }

        CapNhatGiaoDien();
    }

    private void CapNhatGiaoDien()
    {
        if (IsDisposed)
        {
            return;
        }

        IReadOnlyList<ClientDangKetNoi> danhSach = _mayChu.LayDanhSachClient();
        _lblTrangThai.Text = _mayChu.DangChay
            ? $"Trạng thái: Đang chạy tại 0.0.0.0:{GiaoThuc.CongMacDinh} • {danhSach.Count} kết nối"
            : "Trạng thái: Đã dừng";
        _btnBatDau.Enabled = !_mayChu.DangChay;
        _btnDung.Enabled = _mayChu.DangChay;

        _lvClient.BeginUpdate();
        _lvClient.Items.Clear();
        foreach (ClientDangKetNoi client in danhSach)
        {
            var dong = new ListViewItem(client.MaPhien);
            dong.SubItems.Add(client.TenDangNhap);
            dong.SubItems.Add(client.HetHan.HasValue
                ? client.HetHan.Value.ToLocalTime().ToString("HH:mm:ss dd/MM/yyyy")
                : "—");
            _lvClient.Items.Add(dong);
        }

        _lvClient.EndUpdate();
    }

    private void MayChu_CoLog(string dong)
    {
        if (IsDisposed)
        {
            return;
        }

        if (InvokeRequired)
        {
            try
            {
                BeginInvoke(new Action<string>(MayChu_CoLog), dong);
            }
            catch (ObjectDisposedException)
            {
            }
            catch (InvalidOperationException)
            {
            }

            return;
        }

        _txtLog.AppendText(dong + Environment.NewLine);
        if (_txtLog.TextLength > 150_000)
        {
            _txtLog.Text = _txtLog.Text.Substring(_txtLog.TextLength - 100_000);
        }
    }

    private void FormServer_FormClosing(object? sender, FormClosingEventArgs e)
    {
        _timerCapNhat.Stop();
        _mayChu.CoLog -= MayChu_CoLog;
        _mayChu.DungNhanh();
        _timerCapNhat.Dispose();
    }
}
