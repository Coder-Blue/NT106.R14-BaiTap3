# Tài liệu giao thức – Bài tập 3

## 1. Cổng
TCP **5050** (trong dải 1024–49151). Server nghe ở `IPAddress.Any`.

## 2. Đóng khung thông điệp
Mỗi thông điệp = **4 byte độ dài (big-endian, int32)** + **nội dung JSON mã hoá UTF-8**.
- Độ dài chỉ tính phần nội dung, tối đa 65 536 byte; vượt quá hoặc âm thì ngắt kết nối.
- Cả hai chiều dùng cùng khung. Nội dung được phép chứa ký tự xuống dòng.
- Lý do: TCP là luồng byte, không giữ ranh giới giữa các lần gửi; tiền tố độ dài cho biết phải đọc đúng bao nhiêu byte.

## 3. Cấu trúc thông điệp
Yêu cầu: `{"lenh":"<MÃ LỆNH>","thamSo":{"khoá":"giá trị"}}`
Phản hồi: `{"ma":<mã lỗi>,"thongBao":"<chuỗi tiếng Việt>","duLieu":{...}}` (`ma` = 0 là thành công).

## 4. Bảng lệnh
| Lệnh | Tham số | Phản hồi thành công | Trạng thái |
|---|---|---|---|
| DANGKY | tenDangNhap, matKhau, hoTen, email (email có thể rỗng) | ma=0, thongBao="Đăng ký thành công." | Đã cài |
| DANGNHAP | tenDangNhap, matKhau | ma=0, duLieu = thông tin người dùng (không có mật khẩu) | Chưa cài |
| LAYTHONGTIN | – | ma=0, duLieu = thông tin người dùng | Chưa cài |
| DANGXUAT | – | ma=0 rồi server đóng phiên | Chưa cài |

Quy tắc dữ liệu DANGKY: tenDangNhap 3–20 ký tự; matKhau ≥ 6 ký tự; hoTen 1–50 ký tự; email rỗng hoặc đúng định dạng, ≤ 100 ký tự.

## 5. Bảng mã lỗi
| Mã | Tên | Ý nghĩa |
|---|---|---|
| 0 | THANHCONG | Xử lý thành công |
| 100 | LENH_KHONG_HOP_LE | Yêu cầu không đọc được, thiếu lệnh hoặc lệnh chưa hỗ trợ |
| 101 | THAM_SO_KHONG_HOP_LE | Tham số thiếu hoặc vi phạm quy tắc dữ liệu (thongBao nêu lý do) |
| 200 | TEN_DA_TON_TAI | Tên đăng nhập đã có người dùng (DANGKY) |
| 300 | LOI_CSDL | Server không truy cập được cơ sở dữ liệu |
| 500 | LOI_MAY_CHU | Lỗi nội bộ khác của server |

## 6. Ghi chú
Mật khẩu được client gửi nguyên văn, server băm bằng PBKDF2-HMAC-SHA256 (600 000 vòng, salt 16 byte riêng mỗi tài khoản) trước khi ghi. Server ngắt client sau 5 phút không có dữ liệu.
