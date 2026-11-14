# NT106.R14 - Bài tập 3

Bài tập 3: ứng dụng quản lý người dùng theo mô hình client - server, giao tiếp bằng TCP socket.
Client chỉ hiển thị giao diện; server làm việc với SQL Server và phục vụ nhiều client cùng lúc.

## Chức năng

- Menu chọn mở Client hoặc Server.
- Đăng ký tài khoản: kiểm tra dữ liệu ở client và server, kiểm tra trùng tên, băm mật khẩu bằng PBKDF2.
- Đăng nhập: xác thực bằng hash + salt, trả thông tin người dùng, cấp token 30 phút.
- Trang chính: hiển thị thông tin tài khoản, đăng xuất, đóng bằng nút X cũng gỡ phiên.
- Server: `TcpListener` tại `IPAddress.Any:8888`, mỗi client một tác vụ.

## Cài đặt và chạy

1. Cài .NET 10 SDK, Visual Studio 2022/2026 (workload .NET Desktop) và SQL Server/LocalDB.
2. Mở `NT106.R14-BaiTap3.slnx`, chọn project `Client` làm startup rồi bấm F5.
3. Ở menu chọn `Mở Server` trước, sau đó `Mở Client`. Hoặc chạy:

   ```powershell
   dotnet run --project .\Client
   ```

4. Tạo database bằng SSMS: mở `Server/database.sql` và bấm Execute. Nếu dùng instance khác, sửa `Server/appsettings.json` rồi build lại server.
5. Trên form đăng nhập, nhập IP `127.0.0.1`, cổng `8888`, sau đó đăng ký hoặc đăng nhập.

## Cấu trúc

| Tệp / thư mục | Vai trò |
|---|---|
| `NT106.R14-BaiTap3.slnx` | Solution gồm Client, Server, Shared |
| `Client/` | App WinForms: menu, đăng nhập, đăng ký, trang chính, kết nối TCP |
| `Client/Forms/DangNhap/` | Form đăng nhập và file designer |
| `Client/Forms/DangKy/` | Form đăng ký và file designer |
| `Client/Forms/FormChinh/` | Trang chính và file designer |
| `Client/Forms/MenuChinh/` | Menu chọn mở Client hoặc Server |
| `Server/` | Server console, xử lý SQL, băm mật khẩu, phiên đăng nhập |
| `Server/database.sql` | Script tạo database và bảng `NguoiDung` cho SSMS |
| `Server/appsettings.json` | Chuỗi kết nối SQL Server |
| `Shared/` | Giao thức, record dùng chung, kiểm tra dữ liệu nhập |

## Giao thức

- TCP cổng `8888`, UTF-8, mỗi yêu cầu/phản hồi là một dòng JSON kết thúc bằng `\n`.
- Token: chuỗi hex 32 byte, hết hạn sau 30 phút.

| Lệnh | Tham số gửi lên | Phản hồi thành công |
|---|---|---|
| `DANGKY` | `TenDangNhap`, `MatKhau`, `HoTen`, `Email`, `Birthday` | `OK`, `ThongBao` |
| `DANGNHAP` | `TenDangNhap`, `MatKhau` | `OK`, `DuLieuNguoiDung`, `Token`, `HetHanToken` |
| `LAYTHONGTIN` | `Token` | `OK`, `DuLieuNguoiDung` |
| `DANGXUAT` | `Token` | `OK`; server gỡ phiên và đóng kết nối |

`DuLieuNguoiDung` gồm `UserId`, `Username`, `FullName`, `Email`, `Birthday`.

| Mã lỗi | Ý nghĩa |
|---|---|
| `OK` | Xử lý thành công |
| `ERR_TRUNG_TEN` | Tên đăng nhập đã tồn tại |
| `ERR_SAI_THONG_TIN` | Sai tên đăng nhập hoặc mật khẩu |
| `ERR_DU_LIEU` | Dữ liệu không hợp lệ hoặc lệnh không tồn tại |
| `ERR_TOKEN` | Token sai, thiếu hoặc đã hết hạn |
| `ERR_HE_THONG` | Lỗi server; chi tiết chỉ ghi ở log |

## Ghi chú

- Mật khẩu dùng PBKDF2-HMAC-SHA256, 600.000 vòng lặp, salt riêng 16 byte.
- Bảng `dbo.NguoiDung`: `UserId` khóa chính, `Username` UNIQUE, `PasswordHash`, `PasswordSalt`, `Email`, `FullName`, `Birthday`.
- Chỉ server kết nối SQL Server; client không tham chiếu `Microsoft.Data.SqlClient`.
