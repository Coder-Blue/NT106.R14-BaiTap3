using System.IO;
using System.Net.Sockets;

namespace CRUD_APP;

public partial class DangKy : Form
{
    private readonly string _ipServer = "127.0.0.1";
    private readonly int _congServer = GiaoThuc.CongMacDinh;

    public string TenDangNhapDaDangKy { get; private set; } = "";

    public DangKy() => InitializeComponent();

    // Constructor nhận IP và Port từ Form Đăng nhập truyền sang
    public DangKy(string ipServer, int congServer) : this()
    {
        _ipServer = ipServer;
        _congServer = congServer;
    }

    private async void btnDangKy_Click(object sender, EventArgs e)
    {
        if (!KiemTraDuLieuHopLe())
        {
            return;
        }

        btnDangKy.Enabled = false;
        btnDangKy.Text = "Đang xử lý...";
        try
        {
            var tenDangNhap = txtTenDangNhap.Text.Trim();
            var matKhau = txtMatKhau.Text;
            var hoTen = txtHoTen.Text.Trim();
            var email = txtEmail.Text.Trim();

            // Mở kết nối TCP tới Server và gửi gói tin DANGKY
            using var ketNoi = new KetNoiTcp();
            await ketNoi.KetNoiAsync(_ipServer, _congServer);

            var yeuCau = new YeuCau(GiaoThuc.LenhDangKy, tenDangNhap, matKhau, hoTen, email);
            PhanHoi phanHoi = await ketNoi.GuiYeuCauAsync(yeuCau);

            if (!phanHoi.ThanhCong)
            {
                if (phanHoi.MaLoi == GiaoThuc.LoiTrungTen)
                {
                    lblLoiTenDangNhap.Text = "Tên này đã có người dùng.";
                    txtTenDangNhap.Focus();
                }
                else
                {
                    MessageBox.Show(phanHoi.ThongBao, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                return;
            }

            TenDangNhapDaDangKy = tenDangNhap;

            MessageBox.Show("Đăng ký thành công! Bạn có thể đăng nhập ngay.", "Thông báo",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
            Close();
        }
        catch (SocketException)
        {
            MessageBox.Show("Không thể kết nối đến Server. Vui lòng kiểm tra IP, Port hoặc bật Server.", "Lỗi kết nối",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        catch (IOException)
        {
            MessageBox.Show("Kết nối với Server bị gián đoạn. Vui lòng thử lại.", "Lỗi đường truyền",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        finally
        {
            btnDangKy.Enabled = true;
            btnDangKy.Text = "Đăng ký";
        }
    }

    private void btnHuy_Click(object sender, EventArgs e) => Close();

    private bool KiemTraDuLieuHopLe()
    {
        var cacO = new (TextBox Nhap, Label Loi, string? ThongBao)[]
        {
            (txtTenDangNhap, lblLoiTenDangNhap, KiemTraNhapLieu.TenDangNhap(txtTenDangNhap.Text.Trim())),
            (txtMatKhau, lblLoiMatKhau, KiemTraNhapLieu.MatKhau(txtMatKhau.Text)),
            (txtXacNhanMatKhau, lblLoiXacNhanMatKhau, KiemTraNhapLieu.XacNhanMatKhau(txtMatKhau.Text, txtXacNhanMatKhau.Text)),
            (txtHoTen, lblLoiHoTen, KiemTraNhapLieu.HoTen(txtHoTen.Text.Trim())),
            (txtEmail, lblLoiEmail, KiemTraNhapLieu.Email(txtEmail.Text.Trim())),
        };

        foreach (var (_, loi, thongBao) in cacO)
        {
            loi.Text = thongBao ?? "";
        }

        var oLoiDauTien = Array.Find(cacO, o => o.ThongBao is not null);
        oLoiDauTien.Nhap?.Focus();
        return oLoiDauTien.ThongBao is null;
    }
}
