using System.Net.Sockets;
using System.Text.Json;
using BaiTap3.Chung;

namespace BaiTap3.KhachHang;

public partial class DangKy : Form
{
    private static readonly TimeSpan ThoiGianChoToiDa = TimeSpan.FromSeconds(10);
    private static readonly Color MauBinhThuong = Color.FromArgb(110, 110, 110);
    private static readonly Color MauLoi = Color.FromArgb(192, 40, 40);
    private static readonly Color MauThanhCong = Color.FromArgb(40, 140, 70);

    public string TenDangNhapDaDangKy { get; private set; } = "";

    public DangKy()
    {
        InitializeComponent();
        txtDiaChi.Text = "127.0.0.1";
        txtCong.Text = GiaoThuc.CongMacDinh.ToString();
        DatTrangThai("Chưa kết nối", MauBinhThuong);
    }

    private async void btnDangKy_Click(object sender, EventArgs e)
    {
        if (!KiemTraDuLieuHopLe() || !DocCauHinhMayChu(out var diaChi, out var cong))
        {
            return;
        }

        var tenDangNhap = txtTenDangNhap.Text.Trim();
        var yeuCau = new YeuCau(GiaoThuc.Lenh.DangKy, new Dictionary<string, string>
        {
            [GiaoThuc.Khoa.TenDangNhap] = tenDangNhap,
            [GiaoThuc.Khoa.MatKhau] = txtMatKhau.Text, // gửi nguyên văn, server mới là nơi băm
            [GiaoThuc.Khoa.HoTen] = txtHoTen.Text.Trim(),
            [GiaoThuc.Khoa.Email] = txtEmail.Text.Trim(),
        });

        DatDangCho(true);
        try
        {
            using var cts = new CancellationTokenSource(ThoiGianChoToiDa);
            using var ketNoi = new KetNoiMayChu();

            DatTrangThai("Đang kết nối...", MauBinhThuong);
            await ketNoi.KetNoiAsync(diaChi, cong, cts.Token);

            DatTrangThai("Đã kết nối, đang gửi yêu cầu...", MauThanhCong);
            var phanHoi = await ketNoi.GuiAsync(yeuCau, cts.Token);

            XuLyPhanHoi(phanHoi, tenDangNhap);
        }
        catch (SocketException)
        {
            BaoLoiMang("Không kết nối được đến máy chủ. Hãy kiểm tra địa chỉ IP, cổng và xem máy chủ đã chạy chưa.");
        }
        catch (OperationCanceledException)
        {
            BaoLoiMang("Máy chủ không phản hồi kịp thời. Vui lòng thử lại.");
        }
        catch (IOException)
        {
            BaoLoiMang("Mất kết nối với máy chủ. Vui lòng thử lại.");
        }
        catch (InvalidDataException)
        {
            BaoLoiMang("Máy chủ trả về dữ liệu không đúng định dạng.");
        }
        catch (JsonException)
        {
            BaoLoiMang("Máy chủ trả về dữ liệu không đúng định dạng.");
        }
        finally
        {
            DatDangCho(false);
        }
    }

    private void XuLyPhanHoi(PhanHoi phanHoi, string tenDangNhap)
    {
        switch (phanHoi.Ma)
        {
            case MaLoi.ThanhCong:
                TenDangNhapDaDangKy = tenDangNhap;
                MessageBox.Show("Đăng ký thành công! Bạn có thể đăng nhập ngay.", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                Close();
                break;

            case MaLoi.TenDaTonTai:
                lblLoiTenDangNhap.Text = "Tên này đã có người dùng.";
                txtTenDangNhap.Focus();
                DatTrangThai("Đã kết nối", MauThanhCong);
                break;

            default:
                // Server chỉ gửi thông báo tiếng Việt dành cho người dùng, không chứa tên ngoại lệ.
                MessageBox.Show(phanHoi.ThongBao, "Không thể đăng ký", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                DatTrangThai("Đã kết nối", MauThanhCong);
                break;
        }
    }

    private void btnHuy_Click(object sender, EventArgs e) => Close();

    private void BaoLoiMang(string thongBao)
    {
        DatTrangThai("Lỗi kết nối", MauLoi);
        MessageBox.Show(thongBao, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
    }

    private void DatDangCho(bool dangCho)
    {
        btnDangKy.Enabled = !dangCho;
        txtDiaChi.Enabled = !dangCho;
        txtCong.Enabled = !dangCho;
        btnDangKy.Text = dangCho ? "Đang xử lý..." : "Đăng ký";
    }

    private void DatTrangThai(string noiDung, Color mau)
    {
        lblTrangThai.Text = "Trạng thái: " + noiDung;
        lblTrangThai.ForeColor = mau;
    }

    private bool DocCauHinhMayChu(out string diaChi, out int cong)
    {
        diaChi = txtDiaChi.Text.Trim();
        if (diaChi.Length == 0)
        {
            DatTrangThai("Chưa nhập địa chỉ máy chủ", MauLoi);
            txtDiaChi.Focus();
            cong = 0;
            return false;
        }

        if (!int.TryParse(txtCong.Text.Trim(), out cong) || cong is < 1 or > 65535)
        {
            DatTrangThai("Cổng phải là số từ 1 đến 65535", MauLoi);
            txtCong.Focus();
            return false;
        }

        return true;
    }

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
