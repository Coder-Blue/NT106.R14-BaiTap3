using System.Net.Sockets;
using NT106.R14.BaiTap3.Shared;

namespace NT106.R14.BaiTap3.Client.Forms;

public partial class DangKy : Form
{
    private readonly string ipServer;
    private readonly int congServer;

    public string TenDangNhapDaDangKy = "";

    public DangKy()
    {
        InitializeComponent();
        ipServer = GiaoThuc.DiaChiMacDinh;
        congServer = GiaoThuc.CongMacDinh;
        dtpNgaySinh.MaxDate = DateTime.Today;
        dtpNgaySinh.Value = new DateTime(2000, 1, 1);
    }

    public DangKy(string ipServer, int congServer) : this()
    {
        this.ipServer = ipServer;
        this.congServer = congServer;
    }

    private async void btnDangKy_Click(object? sender, EventArgs e)
    {
        if (!KiemTraDuLieuHopLe())
        {
            return;
        }

        string tenDangNhap = txtTenDangNhap.Text.Trim();
        string matKhau = txtMatKhau.Text;
        string hoTen = txtHoTen.Text.Trim();
        string email = txtEmail.Text.Trim();
        DateTime ngaySinh = dtpNgaySinh.Value.Date;

        BatGiaoDien(false);
        var ketNoi = new KetNoiTcp();

        try
        {
            await ketNoi.KetNoiAsync(ipServer, congServer);
            PhanHoi phanHoi = await ketNoi.GuiYeuCauAsync(
                new YeuCau(GiaoThuc.LenhDangKy, tenDangNhap, matKhau, hoTen, email, ngaySinh));

            if (!phanHoi.ThanhCong)
            {
                if (phanHoi.MaLoi == GiaoThuc.LoiTrungTen)
                {
                    lblLoiTenDangNhap.Text = phanHoi.ThongBao;
                    txtTenDangNhap.Focus();
                    return;
                }

                MessageBox.Show(this, phanHoi.ThongBao, "Đăng ký thất bại", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            TenDangNhapDaDangKy = tenDangNhap;
            MessageBox.Show(this, phanHoi.ThongBao, "Đăng ký thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
            DialogResult = DialogResult.OK;
            Close();
        }
        catch (OperationCanceledException)
        {
            MessageBox.Show(this, "Server không phản hồi. Vui lòng thử lại.", "Hết thời gian",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
        catch (SocketException)
        {
            MessageBox.Show(this, "Không thể kết nối đến server. Vui lòng kiểm tra IP, cổng và bật server.",
                "Lỗi kết nối", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        catch (IOException)
        {
            MessageBox.Show(this, "Kết nối với server bị gián đoạn. Vui lòng thử lại.",
                "Lỗi đường truyền", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

    private bool KiemTraDuLieuHopLe()
    {
        string tenDangNhap = txtTenDangNhap.Text.Trim();
        string matKhau = txtMatKhau.Text;
        string xacNhan = txtXacNhanMatKhau.Text;
        string hoTen = txtHoTen.Text.Trim();
        string email = txtEmail.Text.Trim();
        DateTime ngaySinh = dtpNgaySinh.Value.Date;

        string? loiTenDangNhap = KiemTraNhapLieu.TenDangNhap(tenDangNhap);
        string? loiMatKhau = KiemTraNhapLieu.MatKhau(matKhau);
        string? loiXacNhan = KiemTraNhapLieu.XacNhanMatKhau(matKhau, xacNhan);
        string? loiHoTen = KiemTraNhapLieu.HoTen(hoTen);
        string? loiEmail = KiemTraNhapLieu.Email(email);
        string? loiNgaySinh = KiemTraNhapLieu.NgaySinh(ngaySinh);

        lblLoiTenDangNhap.Text = loiTenDangNhap ?? "";
        lblLoiMatKhau.Text = loiMatKhau ?? "";
        lblLoiXacNhanMatKhau.Text = loiXacNhan ?? "";
        lblLoiHoTen.Text = loiHoTen ?? "";
        lblLoiEmail.Text = loiEmail ?? "";
        lblLoiNgaySinh.Text = loiNgaySinh ?? "";

        if (loiTenDangNhap is not null)
        {
            txtTenDangNhap.Focus();
            return false;
        }

        if (loiMatKhau is not null)
        {
            txtMatKhau.Focus();
            return false;
        }

        if (loiXacNhan is not null)
        {
            txtXacNhanMatKhau.Focus();
            return false;
        }

        if (loiHoTen is not null)
        {
            txtHoTen.Focus();
            return false;
        }

        if (loiEmail is not null)
        {
            txtEmail.Focus();
            return false;
        }

        if (loiNgaySinh is not null)
        {
            dtpNgaySinh.Focus();
            return false;
        }

        return true;
    }

    private void BatGiaoDien(bool bat)
    {
        btnDangKy.Enabled = bat;
        btnDangKy.Text = bat ? "Đăng ký" : "Đang xử lý...";
        btnHuy.Enabled = bat;
        txtTenDangNhap.Enabled = bat;
        txtMatKhau.Enabled = bat;
        txtXacNhanMatKhau.Enabled = bat;
        txtHoTen.Enabled = bat;
        txtEmail.Enabled = bat;
        dtpNgaySinh.Enabled = bat;
    }

    private void btnHuy_Click(object? sender, EventArgs e) => Close();
}
