using System.Net.Sockets;
using NT106.R14.BaiTap3.Shared;

namespace NT106.R14.BaiTap3.Client.Forms;

public partial class FormChinh : Form
{
    private KetNoiTcp? ketNoi;
    private string token = "";
    private bool daDongKetNoi;
    private System.Windows.Forms.Timer? timerPhien;
    private DateTime thoiGianHetHan = DateTime.MaxValue;
    private bool dangGiaHan;
    private DateTime lanGiaHanTiepTheo = DateTime.MinValue;
    private string? thongBaoKetThuc;

    public FormChinh() => InitializeComponent();

    public FormChinh(NguoiDung nguoiDung, KetNoiTcp ketNoi, string token, DateTime? hetHan) : this()
    {
        this.ketNoi = ketNoi;
        this.token = token;
        thoiGianHetHan = hetHan ?? DateTime.MaxValue;

        HienThiNguoiDung(nguoiDung);

        if (hetHan is not null)
        {
            timerPhien = new System.Windows.Forms.Timer { Interval = 1000 };
            timerPhien.Tick += TimerPhien_Tick;
            timerPhien.Start();
            CapNhatTrangThai();
        }
        else
        {
            lblTrangThai.Text = "Trạng thái: Đã kết nối";
        }

        FormClosing += FormChinh_FormClosing;
    }

    public string? ThongBaoKetThuc => thongBaoKetThuc;

    private void HienThiNguoiDung(NguoiDung nguoiDung)
    {
        lblChaoMung.Text = $"Xin chào, {nguoiDung.FullName}!";
        lblTenDangNhap.Text = nguoiDung.Username;
        lblHoTen.Text = nguoiDung.FullName;
        lblEmail.Text = HienThi(nguoiDung.Email);
        lblNgaySinh.Text = nguoiDung.Birthday.ToString("dd/MM/yyyy");
        lblUserId.Text = nguoiDung.UserId.ToString();
    }

    private static string HienThi(string? giaTri) =>
        string.IsNullOrEmpty(giaTri) ? "(không có)" : giaTri;

    protected override async void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        if (ketNoi is null || token.Length == 0)
        {
            return;
        }

        btnDangXuat.Enabled = false;
        try
        {
            PhanHoi phanHoi = await ketNoi.GuiYeuCauAsync(
                new YeuCau(GiaoThuc.LenhLayThongTin, Token: token));

            if (IsDisposed || daDongKetNoi)
            {
                return;
            }

            if (phanHoi is { ThanhCong: true, DuLieuNguoiDung: not null })
            {
                HienThiNguoiDung(phanHoi.DuLieuNguoiDung);
                return;
            }

            if (phanHoi.MaLoi == GiaoThuc.LoiHetHanToken)
            {
                KetThucPhien(ThongBaoLoi(phanHoi));
                return;
            }

            string thongBao = string.IsNullOrWhiteSpace(phanHoi.ThongBao)
                ? "Không tải được thông tin người dùng. Vui lòng thử lại."
                : phanHoi.ThongBao;
            MessageBox.Show(this, thongBao, "Trang chính",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
        catch (OperationCanceledException)
        {
            KetThucPhien("Không nhận được phản hồi từ server. Vui lòng đăng nhập lại.");
        }
        catch (SocketException)
        {
            KetThucPhien("Mất kết nối tới server. Vui lòng đăng nhập lại.");
        }
        catch (IOException)
        {
            KetThucPhien("Mất kết nối tới server. Vui lòng đăng nhập lại.");
        }
        catch (ObjectDisposedException)
        {
        }
        finally
        {
            if (!IsDisposed)
            {
                btnDangXuat.Enabled = true;
            }
        }
    }

    private async void TimerPhien_Tick(object? sender, EventArgs e)
    {
        if (IsDisposed || daDongKetNoi || thoiGianHetHan == DateTime.MaxValue)
        {
            return;
        }

        TimeSpan conLai = thoiGianHetHan - DateTime.UtcNow;
        if (conLai <= TimeSpan.Zero)
        {
            timerPhien?.Stop();
            KetThucPhien("Phiên đăng nhập đã hết hạn. Vui lòng đăng nhập lại.");
            return;
        }

        CapNhatTrangThai();

        if (conLai.TotalMinutes <= 5 && !dangGiaHan && DateTime.UtcNow >= lanGiaHanTiepTheo)
        {
            await GiaHanPhienAsync();
        }
    }

    private void CapNhatTrangThai()
    {
        if (thoiGianHetHan == DateTime.MaxValue)
        {
            return;
        }

        if (DateTime.UtcNow < lanGiaHanTiepTheo)
        {
            lblTrangThai.Text = "Trạng thái: Đã kết nối • Chưa gia hạn được phiên, sẽ thử lại...";
            return;
        }

        TimeSpan conLai = thoiGianHetHan - DateTime.UtcNow;
        if (conLai < TimeSpan.Zero)
        {
            conLai = TimeSpan.Zero;
        }

        string thoiGian = conLai.TotalHours >= 1
            ? conLai.ToString(@"h\:mm\:ss")
            : conLai.ToString(@"mm\:ss");
        lblTrangThai.Text = $"Trạng thái: Đã kết nối • Còn lại: {thoiGian}";
    }

    private async Task GiaHanPhienAsync()
    {
        if (ketNoi is null || daDongKetNoi)
        {
            return;
        }

        dangGiaHan = true;
        try
        {
            PhanHoi phanHoi = await ketNoi.GuiYeuCauAsync(
                new YeuCau(GiaoThuc.LenhGiaHanToken, Token: token));

            if (IsDisposed || daDongKetNoi)
            {
                return;
            }

            if (phanHoi.ThanhCong && phanHoi.Token.Length > 0 && phanHoi.HetHanToken.HasValue)
            {
                token = phanHoi.Token;
                thoiGianHetHan = phanHoi.HetHanToken.Value;
                lanGiaHanTiepTheo = DateTime.MinValue;
                CapNhatTrangThai();
                return;
            }

            if (phanHoi.MaLoi == GiaoThuc.LoiHetHanToken)
            {
                KetThucPhien(ThongBaoLoi(phanHoi));
                return;
            }

            lanGiaHanTiepTheo = DateTime.UtcNow.AddSeconds(10);
            CapNhatTrangThai();
        }
        catch (OperationCanceledException)
        {
            KetThucPhien("Không nhận được phản hồi từ server. Vui lòng đăng nhập lại.");
        }
        catch (SocketException)
        {
            KetThucPhien("Mất kết nối tới server. Vui lòng đăng nhập lại.");
        }
        catch (IOException)
        {
            KetThucPhien("Mất kết nối tới server. Vui lòng đăng nhập lại.");
        }
        catch (ObjectDisposedException)
        {
        }
        finally
        {
            dangGiaHan = false;
        }
    }

    private static string ThongBaoLoi(PhanHoi phanHoi) =>
        string.IsNullOrWhiteSpace(phanHoi.ThongBao)
            ? "Phiên đăng nhập không hợp lệ hoặc đã hết hạn. Vui lòng đăng nhập lại."
            : phanHoi.ThongBao;

    private void KetThucPhien(string thongBao)
    {
        if (daDongKetNoi)
        {
            return;
        }

        daDongKetNoi = true;
        thongBaoKetThuc = thongBao;
        timerPhien?.Stop();
        timerPhien?.Dispose();
        timerPhien = null;
        MessageBox.Show(this, thongBao, "Phiên đăng nhập",
            MessageBoxButtons.OK, MessageBoxIcon.Warning);
        ketNoi?.Dispose();
        ketNoi = null;
        Close();
    }

    private async void btnDangXuat_Click(object? sender, EventArgs e)
    {
        btnDangXuat.Enabled = false;
        btnDangXuat.Text = "Đang đăng xuất...";
        timerPhien?.Stop();

        try
        {
            if (!daDongKetNoi && !dangGiaHan && ketNoi is { } && ketNoi.DangKetNoi())
            {
                await ketNoi.GuiYeuCauAsync(new YeuCau(GiaoThuc.LenhDangXuat, Token: token));
            }
        }
        catch (OperationCanceledException)
        {
        }
        catch (SocketException)
        {
        }
        catch (IOException)
        {
        }
        catch (ObjectDisposedException)
        {
        }
        finally
        {
            daDongKetNoi = true;
            ketNoi?.Dispose();
            ketNoi = null;
            Close();
        }
    }

    private void FormChinh_FormClosing(object? sender, FormClosingEventArgs e)
    {
        timerPhien?.Stop();
        timerPhien?.Dispose();
        timerPhien = null;

        if (daDongKetNoi)
        {
            return;
        }

        daDongKetNoi = true;
        if (dangGiaHan)
        {
            ketNoi?.Dispose();
        }
        else
        {
            ketNoi?.GuiNhanhVaDong(new YeuCau(GiaoThuc.LenhDangXuat, Token: token));
        }

        ketNoi = null;
    }
}
