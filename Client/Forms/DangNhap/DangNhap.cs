using System.Net;
using System.Net.Sockets;
using NT106.R14.BaiTap3.Shared;

namespace NT106.R14.BaiTap3.Client.Forms;

public partial class DangNhap : Form
{
    public DangNhap() => InitializeComponent();

    private (bool HopLe, string Ip, int Cong) LayCauHinhServer()
    {
        string ip = txtIp.Text.Trim();
        if (!IPAddress.TryParse(ip, out _))
        {
            lblLoi.Text = "Địa chỉ IP của server không hợp lệ.";
            txtIp.Focus();
            return (false, ip, 0);
        }

        if (!int.TryParse(txtPort.Text.Trim(), out int cong) || cong is < 1024 or > 49151)
        {
            lblLoi.Text = "Cổng server phải là số từ 1024 đến 49151.";
            txtPort.Focus();
            return (false, ip, 0);
        }

        return (true, ip, cong);
    }

    private async void btnDangNhap_Click(object? sender, EventArgs e)
    {
        lblLoi.Text = "";
        var (hopLe, ip, cong) = LayCauHinhServer();
        if (!hopLe)
        {
            return;
        }

        string tenDangNhap = txtTenDangNhap.Text.Trim();
        string matKhau = txtMatKhau.Text;
        string? loi = KiemTraNhapLieu.TenDangNhap(tenDangNhap) ?? KiemTraNhapLieu.MatKhau(matKhau);

        if (loi is not null)
        {
            lblLoi.Text = loi;
            if (tenDangNhap.Length == 0)
            {
                txtTenDangNhap.Focus();
            }
            else
            {
                txtMatKhau.Focus();
            }

            return;
        }

        BatGiaoDien(false);
        lblTrangThaiKetNoi.Text = $"Trạng thái: Đang kết nối tới {ip}:{cong}...";

        var ketNoi = new KetNoiTcp();
        try
        {
            await ketNoi.KetNoiAsync(ip, cong);
            lblTrangThaiKetNoi.Text = $"Trạng thái: Đã kết nối ({ip}:{cong})";

            PhanHoi phanHoi = await ketNoi.GuiYeuCauAsync(new YeuCau(GiaoThuc.LenhDangNhap, tenDangNhap, matKhau));
            if (!phanHoi.ThanhCong || phanHoi.DuLieuNguoiDung is null || phanHoi.Token.Length == 0)
            {
                lblLoi.Text = phanHoi.ThongBao;
                lblTrangThaiKetNoi.Text = "Trạng thái: Đăng nhập thất bại";
                txtMatKhau.Clear();
                txtMatKhau.Focus();
                return;
            }

            MoTrangChinh(phanHoi, ketNoi);
            lblTrangThaiKetNoi.Text = "Trạng thái: Đã đăng xuất";
            txtMatKhau.Clear();
            txtTenDangNhap.Focus();
        }
        catch (OperationCanceledException)
        {
            lblTrangThaiKetNoi.Text = "Trạng thái: Hết thời gian kết nối";
            lblLoi.Text = "Server không phản hồi. Vui lòng thử lại.";
        }
        catch (SocketException)
        {
            lblTrangThaiKetNoi.Text = "Trạng thái: Lỗi kết nối";
            lblLoi.Text = "Không thể kết nối đến server. Vui lòng kiểm tra IP, cổng và bật server.";
        }
        catch (IOException)
        {
            lblTrangThaiKetNoi.Text = "Trạng thái: Mất kết nối";
            lblLoi.Text = "Đường truyền tới server bị gián đoạn.";
        }
        finally
        {
            ketNoi.Dispose();
            if (!IsDisposed)
            {
                BatGiaoDien(true);
            }
        }
    }

    private void MoTrangChinh(PhanHoi phanHoi, KetNoiTcp ketNoi)
    {
        Hide();
        try
        {
            using var formChinh = new FormChinh(
                phanHoi.DuLieuNguoiDung!,
                ketNoi,
                phanHoi.Token,
                phanHoi.HetHanToken);
            formChinh.ShowDialog();
        }
        finally
        {
            Show();
            Activate();
        }
    }

    private void BatGiaoDien(bool bat)
    {
        btnDangNhap.Enabled = bat;
        btnDangNhap.Text = bat ? "Đăng nhập" : "Đang xử lý...";
        btnThoat.Enabled = bat;
        llbDangKy.Enabled = bat;
        txtIp.Enabled = bat;
        txtPort.Enabled = bat;
        txtTenDangNhap.Enabled = bat;
        txtMatKhau.Enabled = bat;
    }

    private void btnThoat_Click(object? sender, EventArgs e) => Close();

    private void llbDangKy_LinkClicked(object? sender, LinkLabelLinkClickedEventArgs e)
    {
        lblLoi.Text = "";
        var (hopLe, ip, cong) = LayCauHinhServer();
        if (!hopLe)
        {
            return;
        }

        using var formDangKy = new DangKy(ip, cong);
        if (formDangKy.ShowDialog(this) == DialogResult.OK && formDangKy.TenDangNhapDaDangKy.Length > 0)
        {
            txtTenDangNhap.Text = formDangKy.TenDangNhapDaDangKy;
            txtMatKhau.Clear();
            txtMatKhau.Focus();
        }
    }
}
