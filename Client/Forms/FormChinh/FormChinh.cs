using System.Net.Sockets;
using NT106.R14.BaiTap3.Shared;

namespace NT106.R14.BaiTap3.Client.Forms;

public partial class FormChinh : Form
{
    private KetNoiTcp? ketNoi;
    private string token = "";
    private bool daDongKetNoi;

    public FormChinh() => InitializeComponent();

    public FormChinh(NguoiDung nguoiDung, KetNoiTcp ketNoi, string token, DateTime? hetHan) : this()
    {
        this.ketNoi = ketNoi;
        this.token = token;

        HienThiNguoiDung(nguoiDung);
        lblTrangThai.Text = hetHan is null
            ? "Trạng thái: Đã kết nối"
            : $"Trạng thái: Đã kết nối • Phiên hết hạn lúc {hetHan.Value.ToLocalTime():HH:mm dd/MM/yyyy}";

        FormClosing += FormChinh_FormClosing;
    }

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

            if (IsDisposed)
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
                MessageBox.Show(this, phanHoi.ThongBao, "Phiên đăng nhập",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                daDongKetNoi = true;
                ketNoi.Dispose();
                Close();
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
            if (!IsDisposed)
            {
                btnDangXuat.Enabled = true;
            }
        }
    }

    private async void btnDangXuat_Click(object? sender, EventArgs e)
    {
        btnDangXuat.Enabled = false;
        btnDangXuat.Text = "Đang đăng xuất...";

        try
        {
            if (!daDongKetNoi && ketNoi is { } && ketNoi.DangKetNoi())
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
            Close();
        }
    }

    private void FormChinh_FormClosing(object? sender, FormClosingEventArgs e)
    {
        if (daDongKetNoi)
        {
            return;
        }

        daDongKetNoi = true;
        ketNoi?.GuiNhanhVaDong(new YeuCau(GiaoThuc.LenhDangXuat, Token: token));
        ketNoi = null;
    }
}
