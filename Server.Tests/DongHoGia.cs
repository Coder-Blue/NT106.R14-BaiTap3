namespace Server.Tests;

internal sealed class DongHoGia : TimeProvider
{
    private DateTimeOffset _bayGio = new(2026, 11, 15, 9, 0, 0, TimeSpan.Zero);

    public override DateTimeOffset GetUtcNow() => _bayGio;

    public void Tien(TimeSpan thoiGian) => _bayGio += thoiGian;
}
