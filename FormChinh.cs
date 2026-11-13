using System.IO;
using System.Net.Sockets;

namespace CRUD_APP;

public partial class FormChinh : Form
{
    private readonly KetNoiTcp? _ketNoi;
    private bool _daDongKetNoi = false;

    public FormChinh() => InitializeComponent();

    public FormChinh(NguoiDung nguoiDung, KetNoiTcp ketNoi) : this()
    {
        _ketNoi = ketNoi;

        lblChaoMung.Text = $"Xin chào, {nguoiDung.HoTen}!";
        lblTenDangNhap.Text = nguoiDung.TenDangNhap;
        lblHoTen.Text = nguoiDung.HoTen;
        lblEmail.Text = HienThi(nguoiDung.Email);
        lblNgayTao.Text = nguoiDung.NgayTao.ToString("dd/MM/yyyy HH:mm");
        lblLanDangNhapCuoi.Text = nguoiDung.LanDangNhapCuoi?.ToString("dd/MM/yyyy HH:mm") ?? "Đăng nhập lần đầu";

        this.FormClosing += FormChinh_FormClosing;
    }

    private static string HienThi(string? giaTri) =>
        string.IsNullOrEmpty(giaTri) ? "(không có)" : giaTri;

    private async void btnDangXuat_Click(object sender, EventArgs e)
    {
        btnDangXuat.Enabled = false;
        btnDangXuat.Text = "Đang xử lý...";

        try
        {
            if (!_daDongKetNoi && _ketNoi is { DangKetNoi: true })
            {
                await _ketNoi.GuiYeuCauAsync(new YeuCau(GiaoThuc.LenhDangXuat));
            }
        }
        catch (SocketException)
        {
        }
        catch (IOException)
        {
        }
        finally
        {
            _daDongKetNoi = true;
            _ketNoi?.Dispose();
            Close(); 
        }
    }

    private void FormChinh_FormClosing(object? sender, FormClosingEventArgs e)
    {
        if (_daDongKetNoi) return;
        _daDongKetNoi = true;

        _ketNoi?.GuiNhanhDangXuatVaDong();
    }

    protected override async void OnLoad(EventArgs e)
    {
        base.OnLoad(e);
        if (_ketNoi is { DangKetNoi: true } && !string.IsNullOrEmpty(lblTenDangNhap.Text))
        {
            try
            {
                var phanHoi = await _ketNoi.GuiYeuCauAsync(
                    new YeuCau(GiaoThuc.LenhLayThongTin, lblTenDangNhap.Text));
                if (phanHoi is { ThanhCong: true, DuLieuNguoiDung: not null })
                {
                    var nd = phanHoi.DuLieuNguoiDung;
                    lblChaoMung.Text = $"Xin chào, {nd.HoTen}!";
                    lblHoTen.Text = nd.HoTen;
                    lblEmail.Text = HienThi(nd.Email);
                    lblNgayTao.Text = nd.NgayTao.ToString("dd/MM/yyyy HH:mm");
                    lblLanDangNhapCuoi.Text = nd.LanDangNhapCuoi?.ToString("dd/MM/yyyy HH:mm") ?? "Đăng nhập lần đầu";
                }
            }
            catch (SocketException) { }
            catch (IOException) { }
        }
    }
}
