using System.Collections.Concurrent;

namespace NT106.R14.BaiTap3.Server;

public sealed class QuanLyPhien
{
    private readonly ConcurrentDictionary<string, PhienDangNhap> _danhSach = new();
    private readonly TimeProvider _thoiGian;
    private readonly Func<string> _taoToken;
    private readonly TimeSpan _thoiHan;

    public QuanLyPhien(
        TimeProvider? thoiGian = null,
        Func<string>? taoToken = null,
        TimeSpan? thoiHan = null)
    {
        _thoiGian = thoiGian ?? TimeProvider.System;
        _taoToken = taoToken ?? MatKhau.TaoToken;
        _thoiHan = thoiHan ?? TimeSpan.FromMinutes(30);
    }

    public int SoPhien => _danhSach.Count;

    private DateTime BayGio => _thoiGian.GetUtcNow().UtcDateTime;

    public void ThemKetNoi(string maPhien) =>
        _danhSach[maPhien] = PhienDangNhap.ChuaDangNhap;

    public PhienDangNhap TaoPhien(string tenDangNhap) =>
        new(tenDangNhap, TaoTokenMoi(), BayGio.Add(_thoiHan));

    public void DatPhien(string maPhien, PhienDangNhap phien) =>
        _danhSach[maPhien] = phien;

    public PhienDangNhap? TimPhien(string maPhien, string token)
    {
        if (token.Length == 0 || !_danhSach.TryGetValue(maPhien, out PhienDangNhap? phien))
        {
            return null;
        }

        if (phien.Token != token || phien.HetHan <= BayGio)
        {
            _danhSach.TryRemove(maPhien, out _);
            return null;
        }

        return phien;
    }

    public PhienDangNhap? GiaHanPhien(string maPhien, string token)
    {
        if (token.Length == 0 || !_danhSach.TryGetValue(maPhien, out PhienDangNhap? phien))
        {
            return null;
        }

        if (phien.Token != token || phien.HetHan <= BayGio)
        {
            _danhSach.TryRemove(maPhien, out _);
            return null;
        }

        var phienMoi = phien with
        {
            Token = TaoTokenMoi(phien.Token),
            HetHan = phien.HetHan.Add(_thoiHan),
        };

        return _danhSach.TryUpdate(maPhien, phienMoi, phien) ? phienMoi : null;
    }

    public PhienDangNhap? XoaPhien(string maPhien) =>
        _danhSach.TryRemove(maPhien, out PhienDangNhap? phien) ? phien : null;

    public IReadOnlyList<ClientDangKetNoi> LayDanhSach() =>
        _danhSach
            .Select(cap => new ClientDangKetNoi(
                cap.Key,
                cap.Value.TenDangNhap,
                cap.Value.Token.Length > 0,
                cap.Value.Token.Length > 0 ? cap.Value.HetHan : null))
            .OrderBy(client => client.MaPhien)
            .ToList();

    public void XoaTatCa() => _danhSach.Clear();

    private string TaoTokenMoi(string? tokenCu = null)
    {
        for (int lan = 0; lan < 10; lan++)
        {
            string token = _taoToken();
            if (token.Length > 0 && token != tokenCu)
            {
                return token;
            }
        }

        throw new InvalidOperationException("Không tạo được token mới.");
    }
}

public sealed record ClientDangKetNoi(
    string MaPhien,
    string TenDangNhap,
    bool DaDangNhap,
    DateTime? HetHan);
