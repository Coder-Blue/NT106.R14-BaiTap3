using System.Collections.Concurrent;

namespace NT106.R14.BaiTap3.Server;

public sealed class KhoaDangNhap
{
    public const int SoLanSaiToiDa = 5;
    private const int NguongDonDep = 10_000;
    private static readonly TimeSpan CuaSo = TimeSpan.FromMinutes(5);
    private static readonly TimeSpan ThoiGianKhoa = TimeSpan.FromMinutes(5);

    private sealed class TrangThai
    {
        public int SoLanSai;
        public DateTime BatDauCuaSo;
        public DateTime KhoaDen;
    }

    private readonly ConcurrentDictionary<string, TrangThai> _danhSach = new();
    private readonly TimeProvider _thoiGian;

    public KhoaDangNhap(TimeProvider? thoiGian = null) =>
        _thoiGian = thoiGian ?? TimeProvider.System;

    private DateTime BayGio => _thoiGian.GetUtcNow().UtcDateTime;

    private static string TaoKhoa(string tenDangNhap, string ip) =>
        $"{tenDangNhap.Trim().ToLowerInvariant()}|{ip}";

    /// <summary>Thời gian còn bị khóa, hoặc null nếu không bị khóa.</summary>
    public TimeSpan? ThoiGianConKhoa(string tenDangNhap, string ip)
    {
        if (!_danhSach.TryGetValue(TaoKhoa(tenDangNhap, ip), out TrangThai? tt))
        {
            return null;
        }

        lock (tt)
        {
            TimeSpan con = tt.KhoaDen - BayGio;
            return con > TimeSpan.Zero ? con : null;
        }
    }

    /// <summary>Ghi nhận một lần sai. KhoaTrong khác null nghĩa là vừa bị khóa.</summary>
    public (int SoLanSai, TimeSpan? KhoaTrong) GhiNhanSai(string tenDangNhap, string ip)
    {
        if (_danhSach.Count > NguongDonDep)
        {
            DonDep();
        }

        TrangThai tt = _danhSach.GetOrAdd(TaoKhoa(tenDangNhap, ip), _ => new TrangThai());
        lock (tt)
        {
            DateTime bayGio = BayGio;
            if (tt.KhoaDen > bayGio)
            {
                return (SoLanSaiToiDa, tt.KhoaDen - bayGio);
            }

            if (tt.SoLanSai == 0 || bayGio - tt.BatDauCuaSo > CuaSo)
            {
                tt.SoLanSai = 0;
                tt.BatDauCuaSo = bayGio;
            }

            tt.SoLanSai++;
            if (tt.SoLanSai >= SoLanSaiToiDa)
            {
                tt.SoLanSai = 0;
                tt.KhoaDen = bayGio + ThoiGianKhoa;
                return (SoLanSaiToiDa, ThoiGianKhoa);
            }

            return (tt.SoLanSai, null);
        }
    }

    public void XoaKhiThanhCong(string tenDangNhap, string ip) =>
        _danhSach.TryRemove(TaoKhoa(tenDangNhap, ip), out _);

    private void DonDep()
    {
        DateTime bayGio = BayGio;
        foreach ((string khoa, TrangThai tt) in _danhSach)
        {
            lock (tt)
            {
                if (tt.KhoaDen <= bayGio && bayGio - tt.BatDauCuaSo > CuaSo)
                {
                    _danhSach.TryRemove(khoa, out _);
                }
            }
        }
    }
}
