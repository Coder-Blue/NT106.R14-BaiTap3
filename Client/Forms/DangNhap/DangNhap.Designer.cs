namespace NT106.R14.BaiTap3.Client.Forms
{
    partial class DangNhap
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
            lblKetNoi = new Label();
            lblIpServer = new Label();
            txtIp = new TextBox();
            lblPortServer = new Label();
            txtPort = new TextBox();
            lblTrangThaiKetNoi = new Label();
            lblTenDangNhap = new Label();
            txtTenDangNhap = new TextBox();
            lblMatKhau = new Label();
            txtMatKhau = new TextBox();
            lblLoi = new Label();
            btnDangNhap = new Button();
            btnThoat = new Button();
            llbDangKy = new LinkLabel();
            SuspendLayout();
            //
            //lblTieuDe
            //
            lblTieuDe.Font = new Font("Segoe UI", 22F, FontStyle.Bold);
            lblTieuDe.ForeColor = Color.FromArgb(35, 78, 120);
            lblTieuDe.Location = new Point(0, 28);
            lblTieuDe.Name = "lblTieuDe";
            lblTieuDe.Size = new Size(600, 44);
            lblTieuDe.TabIndex = 0;
            lblTieuDe.Text = "ĐĂNG NHẬP";
            lblTieuDe.TextAlign = ContentAlignment.MiddleCenter;
            //
            //lblPhuDe
            //
            lblPhuDe.Font = new Font("Segoe UI", 10.5F);
            lblPhuDe.ForeColor = Color.FromArgb(100, 100, 100);
            lblPhuDe.Location = new Point(0, 76);
            lblPhuDe.Name = "lblPhuDe";
            lblPhuDe.Size = new Size(600, 24);
            lblPhuDe.TabIndex = 1;
            lblPhuDe.Text = "Hệ thống quản lý người dùng • NT106.R14";
            lblPhuDe.TextAlign = ContentAlignment.MiddleCenter;
            //
            //lblKetNoi
            //
            lblKetNoi.AutoSize = true;
            lblKetNoi.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            lblKetNoi.ForeColor = Color.FromArgb(60, 60, 60);
            lblKetNoi.Location = new Point(48, 120);
            lblKetNoi.Name = "lblKetNoi";
            lblKetNoi.TabIndex = 2;
            lblKetNoi.Text = "Kết nối server";
            //
            //lblIpServer
            //
            lblIpServer.AutoSize = true;
            lblIpServer.Font = new Font("Segoe UI", 10F);
            lblIpServer.Location = new Point(48, 152);
            lblIpServer.Name = "lblIpServer";
            lblIpServer.TabIndex = 3;
            lblIpServer.Text = "IP:";
            //
            //txtIp
            //
            txtIp.Font = new Font("Segoe UI", 10.5F);
            txtIp.Location = new Point(86, 147);
            txtIp.MaxLength = 45;
            txtIp.Name = "txtIp";
            txtIp.Size = new Size(180, 31);
            txtIp.TabIndex = 4;
            txtIp.Text = "127.0.0.1";
            txtIp.TextAlign = HorizontalAlignment.Center;
            //
            //lblPortServer
            //
            lblPortServer.AutoSize = true;
            lblPortServer.Font = new Font("Segoe UI", 10F);
            lblPortServer.Location = new Point(286, 152);
            lblPortServer.Name = "lblPortServer";
            lblPortServer.TabIndex = 5;
            lblPortServer.Text = "Port:";
            //
            //txtPort
            //
            txtPort.Font = new Font("Segoe UI", 10.5F);
            txtPort.Location = new Point(340, 147);
            txtPort.MaxLength = 5;
            txtPort.Name = "txtPort";
            txtPort.Size = new Size(100, 31);
            txtPort.TabIndex = 6;
            txtPort.Text = "8888";
            txtPort.TextAlign = HorizontalAlignment.Center;
            //
            //lblTrangThaiKetNoi
            //
            lblTrangThaiKetNoi.Font = new Font("Segoe UI", 9F, FontStyle.Italic);
            lblTrangThaiKetNoi.ForeColor = Color.FromArgb(100, 100, 100);
            lblTrangThaiKetNoi.Location = new Point(48, 186);
            lblTrangThaiKetNoi.Name = "lblTrangThaiKetNoi";
            lblTrangThaiKetNoi.Size = new Size(504, 22);
            lblTrangThaiKetNoi.TabIndex = 7;
            lblTrangThaiKetNoi.Text = "Trạng thái: Chưa kết nối";
            //
            //lblTenDangNhap
            //
            lblTenDangNhap.AutoSize = true;
            lblTenDangNhap.Font = new Font("Segoe UI", 10F);
            lblTenDangNhap.Location = new Point(48, 230);
            lblTenDangNhap.Name = "lblTenDangNhap";
            lblTenDangNhap.TabIndex = 8;
            lblTenDangNhap.Text = "Tên đăng nhập";
            //
            //txtTenDangNhap
            //
            txtTenDangNhap.Font = new Font("Segoe UI", 10.5F);
            txtTenDangNhap.Location = new Point(48, 254);
            txtTenDangNhap.MaxLength = 20;
            txtTenDangNhap.Name = "txtTenDangNhap";
            txtTenDangNhap.PlaceholderText = "Nhập tên đăng nhập";
            txtTenDangNhap.Size = new Size(504, 31);
            txtTenDangNhap.TabIndex = 9;
            //
            //lblMatKhau
            //
            lblMatKhau.AutoSize = true;
            lblMatKhau.Font = new Font("Segoe UI", 10F);
            lblMatKhau.Location = new Point(48, 298);
            lblMatKhau.Name = "lblMatKhau";
            lblMatKhau.TabIndex = 10;
            lblMatKhau.Text = "Mật khẩu";
            //
            //txtMatKhau
            //
            txtMatKhau.Font = new Font("Segoe UI", 10.5F);
            txtMatKhau.Location = new Point(48, 322);
            txtMatKhau.MaxLength = 100;
            txtMatKhau.Name = "txtMatKhau";
            txtMatKhau.PasswordChar = '●';
            txtMatKhau.PlaceholderText = "Nhập mật khẩu";
            txtMatKhau.Size = new Size(504, 31);
            txtMatKhau.TabIndex = 11;
            //
            //lblLoi
            //
            lblLoi.Font = new Font("Segoe UI", 9F, FontStyle.Italic);
            lblLoi.ForeColor = Color.FromArgb(192, 40, 40);
            lblLoi.Location = new Point(48, 366);
            lblLoi.Name = "lblLoi";
            lblLoi.Size = new Size(504, 44);
            lblLoi.TabIndex = 12;
            //
            //btnDangNhap
            //
            btnDangNhap.BackColor = Color.FromArgb(51, 122, 183);
            btnDangNhap.Cursor = Cursors.Hand;
            btnDangNhap.FlatAppearance.BorderSize = 0;
            btnDangNhap.FlatAppearance.MouseDownBackColor = Color.FromArgb(40, 96, 145);
            btnDangNhap.FlatAppearance.MouseOverBackColor = Color.FromArgb(64, 138, 204);
            btnDangNhap.FlatStyle = FlatStyle.Flat;
            btnDangNhap.Font = new Font("Segoe UI", 10.5F, FontStyle.Bold);
            btnDangNhap.ForeColor = Color.White;
            btnDangNhap.Location = new Point(48, 424);
            btnDangNhap.Name = "btnDangNhap";
            btnDangNhap.Size = new Size(240, 46);
            btnDangNhap.TabIndex = 13;
            btnDangNhap.Text = "Đăng nhập";
            btnDangNhap.UseVisualStyleBackColor = false;
            btnDangNhap.Click += btnDangNhap_Click;
            //
            //btnThoat
            //
            btnThoat.BackColor = Color.White;
            btnThoat.Cursor = Cursors.Hand;
            btnThoat.FlatAppearance.BorderColor = Color.FromArgb(200, 200, 200);
            btnThoat.FlatAppearance.MouseDownBackColor = Color.FromArgb(225, 225, 225);
            btnThoat.FlatAppearance.MouseOverBackColor = Color.FromArgb(245, 245, 245);
            btnThoat.FlatStyle = FlatStyle.Flat;
            btnThoat.Font = new Font("Segoe UI", 10.5F);
            btnThoat.ForeColor = Color.FromArgb(64, 64, 64);
            btnThoat.Location = new Point(312, 424);
            btnThoat.Name = "btnThoat";
            btnThoat.Size = new Size(240, 46);
            btnThoat.TabIndex = 14;
            btnThoat.Text = "Thoát";
            btnThoat.UseVisualStyleBackColor = false;
            btnThoat.Click += btnThoat_Click;
            //
            //llbDangKy
            //
            llbDangKy.Font = new Font("Segoe UI", 9.75F);
            llbDangKy.Location = new Point(48, 490);
            llbDangKy.Name = "llbDangKy";
            llbDangKy.Size = new Size(504, 24);
            llbDangKy.TabIndex = 15;
            llbDangKy.TabStop = true;
            llbDangKy.Text = "Chưa có tài khoản? Đăng ký ngay";
            llbDangKy.TextAlign = ContentAlignment.MiddleCenter;
            llbDangKy.LinkClicked += llbDangKy_LinkClicked;
            //
            //DangNhap
            //
            AcceptButton = btnDangNhap;
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.White;
            CancelButton = btnThoat;
            ClientSize = new Size(600, 540);
            Controls.Add(lblTieuDe);
            Controls.Add(lblPhuDe);
            Controls.Add(lblKetNoi);
            Controls.Add(lblIpServer);
            Controls.Add(txtIp);
            Controls.Add(lblPortServer);
            Controls.Add(txtPort);
            Controls.Add(lblTrangThaiKetNoi);
            Controls.Add(lblTenDangNhap);
            Controls.Add(txtTenDangNhap);
            Controls.Add(lblMatKhau);
            Controls.Add(txtMatKhau);
            Controls.Add(lblLoi);
            Controls.Add(btnDangNhap);
            Controls.Add(btnThoat);
            Controls.Add(llbDangKy);
            Font = new Font("Segoe UI", 9F);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "DangNhap";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Đăng nhập - Bài tập 3";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label lblTieuDe;
        private Label lblPhuDe;
        private Label lblKetNoi;
        private Label lblIpServer;
        private TextBox txtIp;
        private Label lblPortServer;
        private TextBox txtPort;
        private Label lblTrangThaiKetNoi;
        private Label lblTenDangNhap;
        private TextBox txtTenDangNhap;
        private Label lblMatKhau;
        private TextBox txtMatKhau;
        private Label lblLoi;
        private Button btnDangNhap;
        private Button btnThoat;
        private LinkLabel llbDangKy;
    }
}
