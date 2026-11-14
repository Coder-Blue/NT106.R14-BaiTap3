# NT106.R14 - Bài tập 3

Ứng dụng quản lý người dùng theo mô hình client - server, giao tiếp bằng TCP socket.
Client chỉ hiển thị giao diện; server làm việc với SQL Server và phục vụ nhiều client cùng lúc.

## Chức năng

- Đăng ký tài khoản: kiểm tra dữ liệu ở client và server (gồm cả ngày sinh), kiểm tra trùng tên, băm mật khẩu bằng PBKDF2.
- Đăng nhập: xác thực bằng hash + salt, trả thông tin người dùng (không có mật khẩu), cấp token 30 phút.
- Trang chính: hiển thị thông tin tài khoản, đăng xuất, đóng cửa sổ bằng nút X cũng gỡ phiên.
- Server: `TcpListener` tại `IPAddress.Any:8888`, mỗi client một tác vụ, `ConcurrentDictionary` cho danh sách phiên, dừng bằng `Ctrl+C`.
- Giao thức: JSON theo dòng, UTF-8; xem [docs/GIAO_THUC.md](docs/GIAO_THUC.md).

## Cấu trúc

| Thư mục | Vai trò |
|---|---|
| `Client/` | Ứng dụng WinForms: đăng nhập, đăng ký, trang chính, lớp kết nối TCP |
| `Client/Forms/` | Ba form và file designer |
| `Server/` | Server console, xử lý SQL, băm mật khẩu, phiên đăng nhập |
| `Server/database.sql` | Script tạo database và bảng `NguoiDung` |
| `Shared/` | Giao thức, record dùng chung, kiểm tra dữ liệu nhập |
| `docs/` | Tài liệu giao thức |

## Cài đặt và chạy

Yêu cầu: .NET 10 SDK và SQL Server (LocalDB hoặc SQL Server Express).

1. Tạo database:

   ```powershell
   sqlcmd -S "(localdb)\MSSQLLocalDB" -i .\Server\database.sql
   ```

   Nếu dùng instance khác, đặt biến môi trường `BAITAP3_CONNECTION` trước khi chạy server.

2. Chạy server:

   ```powershell
   dotnet run --project .\Server
   ```

3. Chạy client (mở thêm nhiều cửa sổ nếu muốn thử nhiều kết nối):

   ```powershell
   dotnet run --project .\Client
   ```

4. Trên form đăng nhập, nhập IP `127.0.0.1`, cổng `8888`, sau đó đăng ký hoặc đăng nhập.

## Kiểm thử

| Thao tác | Kết quả mong đợi |
|---|---|
| Đăng ký thiếu trường, email sai, ngày sinh không hợp lệ, mật khẩu không khớp | Hiện lỗi ngay trên form, không gửi yêu cầu |
| Đăng ký trùng tên | `ERR_TRUNG_TEN`, báo dưới ô tên đăng nhập |
| Đăng nhập sai tên hoặc sai mật khẩu | Cùng một thông báo, xóa ô mật khẩu |
| Đăng nhập đúng | Mở trang chính, hiển thị thông tin từ server |
| Đăng xuất hoặc đóng nút X | Server ghi log gỡ phiên, client quay lại đăng nhập |
| Hai client cùng lúc | Server phục vụ độc lập, log số phiên chính xác |
| Tắt server rồi thao tác | Client báo lỗi kết nối, không lộ tên ngoại lệ |

## Ghi chú

- Mật khẩu dùng PBKDF2-HMAC-SHA256, 600.000 vòng lặp, salt riêng 16 byte.
- Bảng `dbo.NguoiDung` gồm `UserId` khóa chính, `Username` UNIQUE, `PasswordHash`, `PasswordSalt`, `Email`, `FullName`, `Birthday`.
- `database.sql` tạo bảng `NguoiDung`; bảng `Users` cũ (nếu có) không còn được server sử dụng.
- Chỉ server kết nối SQL Server; client không tham chiếu `Microsoft.Data.SqlClient`.
