namespace CRUD_APP
{
    partial class DangKy
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null)
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            lblTieuDe = new Label();
            lblPhuDe = new Label();
            lblTenDangNhap = new Label();
            txtTenDangNhap = new TextBox();
            lblLoiTenDangNhap = new Label();
            lblMatKhau = new Label();
            txtMatKhau = new TextBox();
            lblLoiMatKhau = new Label();
            lblXacNhanMatKhau = new Label();
            txtXacNhanMatKhau = new TextBox();
            lblLoiXacNhanMatKhau = new Label();
            lblHoTen = new Label();
            txtHoTen = new TextBox();
            lblLoiHoTen = new Label();
            lblEmail = new Label();
            txtEmail = new TextBox();
            lblLoiEmail = new Label();
            btnDangKy = new Button();
            btnHuy = new Button();
            SuspendLayout();
            // 
            // lblTieuDe
            // 
            lblTieuDe.AutoSize = false;
            lblTieuDe.Font = new Font("Segoe UI", 16F, FontStyle.Bold);
            lblTieuDe.Location = new Point(0, 24);
            lblTieuDe.Name = "lblTieuDe";
            lblTieuDe.Size = new Size(520, 32);
            lblTieuDe.TabIndex = 0;
            lblTieuDe.Text = "ĐĂNG KÝ TÀI KHOẢN";
            lblTieuDe.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // lblPhuDe
            // 
            lblPhuDe.AutoSize = false;
            lblPhuDe.Font = new Font("Segoe UI", 9F);
            lblPhuDe.ForeColor = Color.FromArgb(110, 110, 110);
            lblPhuDe.Location = new Point(0, 58);
            lblPhuDe.Name = "lblPhuDe";
            lblPhuDe.Size = new Size(520, 20);
            lblPhuDe.TabIndex = 1;
            lblPhuDe.Text = "Tạo tài khoản mới để sử dụng hệ thống";
            lblPhuDe.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // lblTenDangNhap
            // 
            lblTenDangNhap.AutoSize = true;
            lblTenDangNhap.Font = new Font("Segoe UI", 10F);
            lblTenDangNhap.Location = new Point(48, 100);
            lblTenDangNhap.Name = "lblTenDangNhap";
            lblTenDangNhap.TabIndex = 2;
            lblTenDangNhap.Text = "Tên đăng nhập";
            // 
            // txtTenDangNhap
            // 
            txtTenDangNhap.Font = new Font("Segoe UI", 11F);
            txtTenDangNhap.Location = new Point(48, 122);
            txtTenDangNhap.Name = "txtTenDangNhap";
            txtTenDangNhap.PlaceholderText = "Từ 3 đến 20 ký tự";
            txtTenDangNhap.Size = new Size(424, 27);
            txtTenDangNhap.TabIndex = 3;
            // 
            // lblLoiTenDangNhap
            // 
            lblLoiTenDangNhap.AutoSize = false;
            lblLoiTenDangNhap.Font = new Font("Segoe UI", 8.25F, FontStyle.Italic);
            lblLoiTenDangNhap.ForeColor = Color.FromArgb(192, 40, 40);
            lblLoiTenDangNhap.Location = new Point(48, 152);
            lblLoiTenDangNhap.Name = "lblLoiTenDangNhap";
            lblLoiTenDangNhap.Size = new Size(424, 18);
            lblLoiTenDangNhap.TabIndex = 4;
            // 
            // lblMatKhau
            // 
            lblMatKhau.AutoSize = true;
            lblMatKhau.Font = new Font("Segoe UI", 10F);
            lblMatKhau.Location = new Point(48, 178);
            lblMatKhau.Name = "lblMatKhau";
            lblMatKhau.TabIndex = 5;
            lblMatKhau.Text = "Mật khẩu";
            // 
            // txtMatKhau
            // 
            txtMatKhau.Font = new Font("Segoe UI", 11F);
            txtMatKhau.Location = new Point(48, 200);
            txtMatKhau.Name = "txtMatKhau";
            txtMatKhau.PasswordChar = '●';
            txtMatKhau.PlaceholderText = "Tối thiểu 6 ký tự";
            txtMatKhau.Size = new Size(424, 27);
            txtMatKhau.TabIndex = 6;
            // 
            // lblLoiMatKhau
            // 
            lblLoiMatKhau.AutoSize = false;
            lblLoiMatKhau.Font = new Font("Segoe UI", 8.25F, FontStyle.Italic);
            lblLoiMatKhau.ForeColor = Color.FromArgb(192, 40, 40);
            lblLoiMatKhau.Location = new Point(48, 230);
            lblLoiMatKhau.Name = "lblLoiMatKhau";
            lblLoiMatKhau.Size = new Size(424, 18);
            lblLoiMatKhau.TabIndex = 7;
            // 
            // lblXacNhanMatKhau
            // 
            lblXacNhanMatKhau.AutoSize = true;
            lblXacNhanMatKhau.Font = new Font("Segoe UI", 10F);
            lblXacNhanMatKhau.Location = new Point(48, 256);
            lblXacNhanMatKhau.Name = "lblXacNhanMatKhau";
            lblXacNhanMatKhau.TabIndex = 8;
            lblXacNhanMatKhau.Text = "Xác nhận mật khẩu";
            // 
            // txtXacNhanMatKhau
            // 
            txtXacNhanMatKhau.Font = new Font("Segoe UI", 11F);
            txtXacNhanMatKhau.Location = new Point(48, 278);
            txtXacNhanMatKhau.Name = "txtXacNhanMatKhau";
            txtXacNhanMatKhau.PasswordChar = '●';
            txtXacNhanMatKhau.PlaceholderText = "Nhập lại mật khẩu";
            txtXacNhanMatKhau.Size = new Size(424, 27);
            txtXacNhanMatKhau.TabIndex = 9;
            // 
            // lblLoiXacNhanMatKhau
            // 
            lblLoiXacNhanMatKhau.AutoSize = false;
            lblLoiXacNhanMatKhau.Font = new Font("Segoe UI", 8.25F, FontStyle.Italic);
            lblLoiXacNhanMatKhau.ForeColor = Color.FromArgb(192, 40, 40);
            lblLoiXacNhanMatKhau.Location = new Point(48, 308);
            lblLoiXacNhanMatKhau.Name = "lblLoiXacNhanMatKhau";
            lblLoiXacNhanMatKhau.Size = new Size(424, 18);
            lblLoiXacNhanMatKhau.TabIndex = 10;
            // 
            // lblHoTen
            // 
            lblHoTen.AutoSize = true;
            lblHoTen.Font = new Font("Segoe UI", 10F);
            lblHoTen.Location = new Point(48, 334);
            lblHoTen.Name = "lblHoTen";
            lblHoTen.TabIndex = 11;
            lblHoTen.Text = "Họ tên";
            // 
            // txtHoTen
            // 
            txtHoTen.Font = new Font("Segoe UI", 11F);
            txtHoTen.Location = new Point(48, 356);
            txtHoTen.Name = "txtHoTen";
            txtHoTen.PlaceholderText = "Tối đa 50 ký tự";
            txtHoTen.Size = new Size(424, 27);
            txtHoTen.TabIndex = 12;
            // 
            // lblLoiHoTen
            // 
            lblLoiHoTen.AutoSize = false;
            lblLoiHoTen.Font = new Font("Segoe UI", 8.25F, FontStyle.Italic);
            lblLoiHoTen.ForeColor = Color.FromArgb(192, 40, 40);
            lblLoiHoTen.Location = new Point(48, 386);
            lblLoiHoTen.Name = "lblLoiHoTen";
            lblLoiHoTen.Size = new Size(424, 18);
            lblLoiHoTen.TabIndex = 13;
            // 
            // lblEmail
            // 
            lblEmail.AutoSize = true;
            lblEmail.Font = new Font("Segoe UI", 10F);
            lblEmail.Location = new Point(48, 412);
            lblEmail.Name = "lblEmail";
            lblEmail.TabIndex = 14;
            lblEmail.Text = "Email (không bắt buộc)";
            // 
            // txtEmail
            // 
            txtEmail.Font = new Font("Segoe UI", 11F);
            txtEmail.Location = new Point(48, 434);
            txtEmail.Name = "txtEmail";
            txtEmail.PlaceholderText = "ten@mien.com";
            txtEmail.Size = new Size(424, 27);
            txtEmail.TabIndex = 15;
            // 
            // lblLoiEmail
            // 
            lblLoiEmail.AutoSize = false;
            lblLoiEmail.Font = new Font("Segoe UI", 8.25F, FontStyle.Italic);
            lblLoiEmail.ForeColor = Color.FromArgb(192, 40, 40);
            lblLoiEmail.Location = new Point(48, 464);
            lblLoiEmail.Name = "lblLoiEmail";
            lblLoiEmail.Size = new Size(424, 18);
            lblLoiEmail.TabIndex = 16;
            // 
            // btnDangKy
            // 
            btnDangKy.BackColor = Color.FromArgb(51, 122, 183);
            btnDangKy.Cursor = Cursors.Hand;
            btnDangKy.FlatAppearance.BorderSize = 0;
            btnDangKy.FlatAppearance.MouseDownBackColor = Color.FromArgb(40, 96, 145);
            btnDangKy.FlatAppearance.MouseOverBackColor = Color.FromArgb(64, 138, 204);
            btnDangKy.FlatStyle = FlatStyle.Flat;
            btnDangKy.Font = new Font("Segoe UI", 10.5F, FontStyle.Bold);
            btnDangKy.ForeColor = Color.White;
            btnDangKy.Location = new Point(48, 502);
            btnDangKy.Name = "btnDangKy";
            btnDangKy.Size = new Size(200, 40);
            btnDangKy.TabIndex = 17;
            btnDangKy.Text = "Đăng ký";
            btnDangKy.UseVisualStyleBackColor = false;
            btnDangKy.Click += btnDangKy_Click;
            // 
            // btnHuy
            // 
            btnHuy.BackColor = Color.White;
            btnHuy.Cursor = Cursors.Hand;
            btnHuy.FlatAppearance.BorderColor = Color.FromArgb(200, 200, 200);
            btnHuy.FlatAppearance.MouseDownBackColor = Color.FromArgb(225, 225, 225);
            btnHuy.FlatAppearance.MouseOverBackColor = Color.FromArgb(245, 245, 245);
            btnHuy.FlatStyle = FlatStyle.Flat;
            btnHuy.Font = new Font("Segoe UI", 10.5F);
            btnHuy.ForeColor = Color.FromArgb(64, 64, 64);
            btnHuy.Location = new Point(272, 502);
            btnHuy.Name = "btnHuy";
            btnHuy.Size = new Size(200, 40);
            btnHuy.TabIndex = 18;
            btnHuy.Text = "Huỷ";
            btnHuy.UseVisualStyleBackColor = false;
            btnHuy.Click += btnHuy_Click;
            // 
            // DangKy
            // 
            AcceptButton = btnDangKy;
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.White;
            CancelButton = btnHuy;
            ClientSize = new Size(520, 560);
            Controls.Add(lblTieuDe);
            Controls.Add(lblPhuDe);
            Controls.Add(lblTenDangNhap);
            Controls.Add(txtTenDangNhap);
            Controls.Add(lblLoiTenDangNhap);
            Controls.Add(lblMatKhau);
            Controls.Add(txtMatKhau);
            Controls.Add(lblLoiMatKhau);
            Controls.Add(lblXacNhanMatKhau);
            Controls.Add(txtXacNhanMatKhau);
            Controls.Add(lblLoiXacNhanMatKhau);
            Controls.Add(lblHoTen);
            Controls.Add(txtHoTen);
            Controls.Add(lblLoiHoTen);
            Controls.Add(lblEmail);
            Controls.Add(txtEmail);
            Controls.Add(lblLoiEmail);
            Controls.Add(btnDangKy);
            Controls.Add(btnHuy);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "DangKy";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Đăng ký tài khoản";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label lblTieuDe;
        private Label lblPhuDe;
        private Label lblTenDangNhap;
        private TextBox txtTenDangNhap;
        private Label lblLoiTenDangNhap;
        private Label lblMatKhau;
        private TextBox txtMatKhau;
        private Label lblLoiMatKhau;
        private Label lblXacNhanMatKhau;
        private TextBox txtXacNhanMatKhau;
        private Label lblLoiXacNhanMatKhau;
        private Label lblHoTen;
        private TextBox txtHoTen;
        private Label lblLoiHoTen;
        private Label lblEmail;
        private TextBox txtEmail;
        private Label lblLoiEmail;
        private Button btnDangKy;
        private Button btnHuy;
    }
}
