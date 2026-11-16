# NT106.R14 - Bài tập 3

Ứng dụng quản lý người dùng client-server qua TCP socket. Client chỉ hiển thị giao diện; server xử lý SQL Server và phục vụ nhiều client.

## Chức năng

- Đăng ký, đăng nhập, đăng xuất, xem thông tin người dùng.
- PBKDF2 + salt; token 30 phút, tự gia hạn khi còn dưới 5 phút.
- Tạm khóa sau 5 lần sai trong 5 phút.
- Dò tìm server bằng UDP broadcast (9999).
- Dashboard server: danh sách client, log thời gian, Bắt đầu/Dừng.

## Chạy nhanh

1. Cài .NET 10 SDK + SQL Server/LocalDB; chạy `Server/database.sql`.
2. Mở `NT106.R14-BaiTap3.slnx`, chọn `Client`, bấm F5.
3. Menu: `Mở Server` → `Mở Client`; nhập `127.0.0.1:8888` hoặc bấm `Dò tìm Server`.

```powershell
dotnet run --project .\Server
dotnet run --project .\Client
```

## Giao thức

- TCP `8888`, UDP discovery `9999`, UTF-8; mỗi thông điệp là một dòng JSON kết thúc `\n`.
- Token hex 32 byte, hạn 30 phút; gia hạn tạo token mới và cộng 30 phút.

| Lệnh | Tham số gửi lên | Phản hồi thành công |
|---|---|---|
| `DANGKY` | `TenDangNhap`, `MatKhau`, `HoTen`, `Email`, `Birthday` | `OK`, `ThongBao` |
| `DANGNHAP` | `TenDangNhap`, `MatKhau` | `OK`, `DuLieuNguoiDung`, `Token`, `HetHanToken` |
| `LAYTHONGTIN` | `Token` | `OK`, `DuLieuNguoiDung` |
| `GIAHANTOKEN` | `Token` | `OK`, `Token` mới, `HetHanToken` mới |
| `DANGXUAT` | `Token` | `OK`; server gỡ phiên và đóng kết nối |

| Mã lỗi | Ý nghĩa |
|---|---|
| `OK` | Xử lý thành công |
| `ERR_TRUNG_TEN` | Tên đăng nhập đã tồn tại |
| `ERR_SAI_THONG_TIN` | Sai tên đăng nhập hoặc mật khẩu |
| `ERR_DU_LIEU` | Dữ liệu không hợp lệ hoặc lệnh không tồn tại |
| `ERR_TOKEN` | Token sai, thiếu hoặc đã hết hạn |
| `ERR_HE_THONG` | Lỗi server; chi tiết chỉ ghi ở log |
| `ERR_TAM_KHOA` | Sai 5 lần trong 5 phút; tạm khóa 5 phút |

## Kiểm thử

```powershell
dotnet test NT106.R14-BaiTap3.slnx
dotnet format NT106.R14-BaiTap3.slnx --verify-no-changes
```

## Ghi chú

- PBKDF2-HMAC-SHA256, 600.000 vòng, salt 16 byte; chỉ server kết nối SQL Server, mọi SQL dùng tham số.
