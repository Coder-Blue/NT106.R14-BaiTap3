using System.IO;
using System.Net.Sockets;

namespace CRUD_APP;

public partial class DangNhap : Form
{
    public DangNhap() => InitializeComponent();

    private bool LayCauHinhServer(out string ip, out int cong)
    {
        ip = txtIp.Text.Trim();
        cong = GiaoThuc.CongMacDinh;

        if (ip.Length == 0)
        {
            lblLoi.Text = "Vui lòng nhập địa chỉ IP của server.";
            txtIp.Focus();
            return false;
        }

        if (!int.TryParse(txtPort.Text.Trim(), out cong) || cong is < 1024 or > 49151)
        {
            lblLoi.Text = "Cổng server phải là số từ 1024 đến 49151.";
            txtPort.Focus();
            return false;
        }

        return true;
    }

    private async void btnDangNhap_Click(object sender, EventArgs e)
    {
        lblLoi.Text = "";
        if (!LayCauHinhServer(out string ip, out int cong))
        {
            return;
        }

        var tenDangNhap = txtTenDangNhap.Text.Trim();
        var matKhau = txtMatKhau.Text;

        if (tenDangNhap.Length == 0)
        {
            lblLoi.Text = "Vui lòng nhập tên đăng nhập.";
            txtTenDangNhap.Focus();
            return;
        }

        if (matKhau.Length == 0)
        {
            lblLoi.Text = "Vui lòng nhập mật khẩu.";
            txtMatKhau.Focus();
            return;
        }

        btnDangNhap.Enabled = false;
        btnDangNhap.Text = "Đang xử lý...";
        lblTrangThaiKetNoi.Text = $"Trạng thái: Đang kết nối tới {ip}:{cong}...";

        var ketNoi = new KetNoiTcp();
        try
        {
            await ketNoi.KetNoiAsync(ip, cong);
            lblTrangThaiKetNoi.Text = $"Trạng thái: Đã kết nối ({ip}:{cong})";

            var yeuCau = new YeuCau(GiaoThuc.LenhDangNhap, tenDangNhap, matKhau);
            PhanHoi phanHoi = await ketNoi.GuiYeuCauAsync(yeuCau);

            if (!phanHoi.ThanhCong || phanHoi.DuLieuNguoiDung is null)
            {
                lblLoi.Text = phanHoi.ThongBao;
                txtMatKhau.Clear();
                txtMatKhau.Focus();
                ketNoi.Dispose();
                lblTrangThaiKetNoi.Text = "Trạng thái: Đã ngắt kết nối";
                return;
            }

            MoTrangChinh(phanHoi.DuLieuNguoiDung, ketNoi);
        }
        catch (SocketException)
        {
            ketNoi.Dispose();
            lblTrangThaiKetNoi.Text = "Trạng thái: Lỗi kết nối";
            lblLoi.Text = "Không thể kết nối đến Server. Vui lòng kiểm tra IP, Port hoặc bật Server.";
        }
        catch (IOException)
        {
            ketNoi.Dispose();
            lblTrangThaiKetNoi.Text = "Trạng thái: Mất kết nối";
            lblLoi.Text = "Đường truyền tới Server bị gián đoạn.";
        }
        finally
        {
            btnDangNhap.Enabled = true;
            btnDangNhap.Text = "Đăng nhập";
        }
    }

    private void MoTrangChinh(NguoiDung nguoiDung, KetNoiTcp ketNoi)
    {
        Hide();
        using (var formChinh = new FormChinh(nguoiDung, ketNoi))
        {
            formChinh.ShowDialog();
        }
        Show();
        lblTrangThaiKetNoi.Text = "Trạng thái: Đã đăng xuất / Ngắt kết nối";
        txtMatKhau.Clear();
        txtTenDangNhap.Focus();
    }

    private void btnThoat_Click(object sender, EventArgs e) => Close();

    private void llbDangKy_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
    {
        lblLoi.Text = "";
        if (!LayCauHinhServer(out string ip, out int cong))
        {
            return;
        }

        using var formDangKy = new DangKy(ip, cong);
        formDangKy.ShowDialog();

        if (formDangKy.TenDangNhapDaDangKy is { Length: > 0 } tenDangNhap)
        {
            txtTenDangNhap.Text = tenDangNhap;
            txtMatKhau.Clear();
        }
    }
}
