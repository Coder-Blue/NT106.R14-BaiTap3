# Giao thức Client - Server

## 1. Thông tin chung

| Thành phần | Giá trị |
|---|---|
| Giao vận | TCP |
| Cổng server | 8888 (trong dải 1024 - 49151) |
| Mã hóa ký tự | UTF-8, không có BOM |
| Khuôn dạng | Mỗi thông điệp là một dòng JSON, kết thúc bằng ký tự `\n` |
| Token | Chuỗi ngẫu nhiên 32 byte dạng hex, hết hạn sau 30 phút |

Server lắng nghe tại `IPAddress.Any:8888`, mỗi client được phục vụ trong một tác vụ riêng.

## 2. Đóng khung thông điệp

TCP là luồng byte, không giữ ranh giới thông điệp. Vì vậy mỗi yêu cầu và phản hồi được đóng gói thành một dòng JSON:

```text
{"MaLenh":"DANGNHAP","TenDangNhap":"an","MatKhau":"123456"}\n
```

Quy ước:

- Dùng `StreamReader`/`StreamWriter` với `UTF-8`, `AutoFlush = true`, `NewLine = "\n"`.
- Không đặt ký tự xuống dòng thật trong nội dung JSON; mọi chuỗi được JSON escape.
- Một dòng chỉ chứa đúng một đối tượng JSON. Dòng trống hoặc JSON sai bị trả về `ERR_DU_LIEU`.
- Khi client đóng kết nối, `ReadLineAsync` trả về `null`; server gỡ phiên trong khối `finally`.

## 3. Bảng mã lệnh

| Lệnh | Tham số gửi lên | Phản hồi thành công |
|---|---|---|
| `DANGKY` | `TenDangNhap`, `MatKhau`, `HoTen`, `Email`, `Birthday` | `OK`, `ThongBao` |
| `DANGNHAP` | `TenDangNhap`, `MatKhau` | `OK`, `DuLieuNguoiDung`, `Token`, `HetHanToken` |
| `LAYTHONGTIN` | `Token` | `OK`, `DuLieuNguoiDung` |
| `DANGXUAT` | `Token` | `OK`; server gỡ phiên và đóng kết nối |

`DuLieuNguoiDung` gồm `UserId`, `Username`, `FullName`, `Email`, `Birthday`. Không bao giờ trả về mật khẩu, chuỗi băm hoặc salt. `Birthday` dùng định dạng ISO 8601 (`yyyy-MM-dd`).

## 4. Bảng mã lỗi

| Mã | Ý nghĩa |
|---|---|
| `OK` | Xử lý thành công |
| `ERR_TRUNG_TEN` | Tên đăng nhập đã tồn tại |
| `ERR_SAI_THONG_TIN` | Sai tên đăng nhập hoặc mật khẩu (dùng chung một thông báo) |
| `ERR_DU_LIEU` | Dữ liệu không hợp lệ hoặc lệnh không tồn tại |
| `ERR_TOKEN` | Token sai, thiếu hoặc đã hết hạn |
| `ERR_HE_THONG` | Server gặp lỗi khi xử lý; chi tiết chỉ ghi ở log server |

## 5. Phiên đăng nhập và token

1. `DANGNHAP` thành công: server sinh token bằng `RandomNumberGenerator`, lưu `(token, Username, HetHan)` theo kết nối hiện tại và trả token cho client.
2. `LAYTHONGTIN` và `DANGXUAT` phải gửi kèm token. Server kiểm tra token thuộc đúng kết nối và còn hạn.
3. Token hết hạn hoặc không khớp: trả `ERR_TOKEN`; client quay về form đăng nhập.
4. `DANGXUAT` hoặc đóng cửa sổ: server gỡ phiên khỏi `ConcurrentDictionary` và ghi log.

## 6. Bảo mật

- Mật khẩu chỉ được băm ở server bằng PBKDF2-HMAC-SHA256, 600.000 vòng lặp, salt ngẫu nhiên 16 byte cho từng tài khoản.
- Mọi câu lệnh SQL dùng tham số.
- Client chỉ mở socket TCP, không kết nối trực tiếp cơ sở dữ liệu.
- Server không gửi nội dung ngoại lệ cho client.

## 7. Ví dụ

Đăng nhập:

```json
{"MaLenh":"DANGNHAP","TenDangNhap":"an","MatKhau":"123456"}
```

Phản hồi:

```json
{"ThanhCong":true,"MaLoi":"OK","ThongBao":"Đăng nhập thành công.","DuLieuNguoiDung":{"UserId":1,"Username":"an","FullName":"Nguyễn Văn An","Email":"an@example.com","Birthday":"2000-01-01T00:00:00"},"Token":"A1B2...","HetHanToken":"2026-11-14T08:30:00Z"}
```
