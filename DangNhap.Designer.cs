namespace CRUD_APP
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
            lblTenDangNhap = new Label();
            txtTenDangNhap = new TextBox();
            lblMatKhau = new Label();
            txtMatKhau = new TextBox();
            lblLoi = new Label();
            btnDangNhap = new Button();
            btnThoat = new Button();
            llbDangKy = new LinkLabel();
            lblIpServer = new Label();
            lblPortServer = new Label();
            lblTrangThaiKetNoi = new Label();
            txtIp = new TextBox();
            txtPort = new TextBox();
            SuspendLayout();
            // 
            // lblTieuDe
            // 
            lblTieuDe.Font = new Font("Segoe UI", 16F, FontStyle.Bold);
            lblTieuDe.Location = new Point(0, 43);
            lblTieuDe.Margin = new Padding(4, 0, 4, 0);
            lblTieuDe.Name = "lblTieuDe";
            lblTieuDe.Size = new Size(600, 53);
            lblTieuDe.TabIndex = 0;
            lblTieuDe.Text = "ĐĂNG NHẬP";
            lblTieuDe.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // lblPhuDe
            // 
            lblPhuDe.Font = new Font("Segoe UI", 9F);
            lblPhuDe.ForeColor = Color.FromArgb(110, 110, 110);
            lblPhuDe.Location = new Point(0, 100);
            lblPhuDe.Margin = new Padding(4, 0, 4, 0);
            lblPhuDe.Name = "lblPhuDe";
            lblPhuDe.Size = new Size(600, 33);
            lblPhuDe.TabIndex = 1;
            lblPhuDe.Text = "Hệ thống quản lý người dùng";
            lblPhuDe.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // lblTenDangNhap
            // 
            lblTenDangNhap.AutoSize = true;
            lblTenDangNhap.Font = new Font("Segoe UI", 10F);
            lblTenDangNhap.Location = new Point(64, 158);
            lblTenDangNhap.Margin = new Padding(4, 0, 4, 0);
            lblTenDangNhap.Name = "lblTenDangNhap";
            lblTenDangNhap.Size = new Size(140, 28);
            lblTenDangNhap.TabIndex = 2;
            lblTenDangNhap.Text = "Tên đăng nhập";
            // 
            // txtTenDangNhap
            // 
            txtTenDangNhap.Font = new Font("Segoe UI", 11F);
            txtTenDangNhap.Location = new Point(64, 195);
            txtTenDangNhap.Margin = new Padding(4, 5, 4, 5);
            txtTenDangNhap.Name = "txtTenDangNhap";
            txtTenDangNhap.PlaceholderText = "Nhập tên đăng nhập";
            txtTenDangNhap.Size = new Size(473, 37);
            txtTenDangNhap.TabIndex = 3;
            // 
            // lblMatKhau
            // 
            lblMatKhau.AutoSize = true;
            lblMatKhau.Font = new Font("Segoe UI", 10F);
            lblMatKhau.Location = new Point(64, 244);
            lblMatKhau.Margin = new Padding(4, 0, 4, 0);
            lblMatKhau.Name = "lblMatKhau";
            lblMatKhau.Size = new Size(94, 28);
            lblMatKhau.TabIndex = 4;
            lblMatKhau.Text = "Mật khẩu";
            // 
            // txtMatKhau
            // 
            txtMatKhau.Font = new Font("Segoe UI", 11F);
            txtMatKhau.Location = new Point(64, 281);
            txtMatKhau.Margin = new Padding(4, 5, 4, 5);
            txtMatKhau.Name = "txtMatKhau";
            txtMatKhau.PasswordChar = '●';
            txtMatKhau.PlaceholderText = "Nhập mật khẩu";
            txtMatKhau.Size = new Size(473, 37);
            txtMatKhau.TabIndex = 5;
            // 
            // lblLoi
            // 
            lblLoi.Font = new Font("Segoe UI", 9F, FontStyle.Italic);
            lblLoi.ForeColor = Color.FromArgb(192, 40, 40);
            lblLoi.Location = new Point(63, 480);
            lblLoi.Margin = new Padding(4, 0, 4, 0);
            lblLoi.Name = "lblLoi";
            lblLoi.Size = new Size(474, 57);
            lblLoi.TabIndex = 6;
            // 
            // btnDangNhap
            // 
            btnDangNhap.BackColor = Color.FromArgb(51, 122, 183);
            btnDangNhap.Cursor = Cursors.Hand;
            btnDangNhap.FlatAppearance.BorderSize = 0;
            btnDangNhap.FlatAppearance.MouseDownBackColor = Color.FromArgb(40, 96, 145);
            btnDangNhap.FlatAppearance.MouseOverBackColor = Color.FromArgb(64, 138, 204);
            btnDangNhap.FlatStyle = FlatStyle.Flat;
            btnDangNhap.Font = new Font("Segoe UI", 10.5F, FontStyle.Bold);
            btnDangNhap.ForeColor = Color.White;
            btnDangNhap.Location = new Point(63, 557);
            btnDangNhap.Margin = new Padding(4, 5, 4, 5);
            btnDangNhap.Name = "btnDangNhap";
            btnDangNhap.Size = new Size(229, 67);
            btnDangNhap.TabIndex = 7;
            btnDangNhap.Text = "Đăng nhập";
            btnDangNhap.UseVisualStyleBackColor = false;
            btnDangNhap.Click += btnDangNhap_Click;
            // 
            // btnThoat
            // 
            btnThoat.BackColor = Color.White;
            btnThoat.Cursor = Cursors.Hand;
            btnThoat.FlatAppearance.BorderColor = Color.FromArgb(200, 200, 200);
            btnThoat.FlatAppearance.MouseDownBackColor = Color.FromArgb(225, 225, 225);
            btnThoat.FlatAppearance.MouseOverBackColor = Color.FromArgb(245, 245, 245);
            btnThoat.FlatStyle = FlatStyle.Flat;
            btnThoat.Font = new Font("Segoe UI", 10.5F);
            btnThoat.ForeColor = Color.FromArgb(64, 64, 64);
            btnThoat.Location = new Point(309, 557);
            btnThoat.Margin = new Padding(4, 5, 4, 5);
            btnThoat.Name = "btnThoat";
            btnThoat.Size = new Size(229, 67);
            btnThoat.TabIndex = 8;
            btnThoat.Text = "Thoát";
            btnThoat.UseVisualStyleBackColor = false;
            btnThoat.Click += btnThoat_Click;
            // 
            // llbDangKy
            // 
            llbDangKy.Font = new Font("Segoe UI", 9.75F);
            llbDangKy.Location = new Point(63, 639);
            llbDangKy.Margin = new Padding(4, 0, 4, 0);
            llbDangKy.Name = "llbDangKy";
            llbDangKy.Size = new Size(474, 33);
            llbDangKy.TabIndex = 9;
            llbDangKy.TabStop = true;
            llbDangKy.Text = "Chưa có tài khoản? Đăng ký ngay";
            llbDangKy.TextAlign = ContentAlignment.MiddleCenter;
            llbDangKy.LinkClicked += llbDangKy_LinkClicked;
            // 
            // lblIpServer
            // 
            lblIpServer.AutoSize = true;
            lblIpServer.Font = new Font("Segoe UI", 10F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lblIpServer.Location = new Point(64, 364);
            lblIpServer.Name = "lblIpServer";
            lblIpServer.Size = new Size(92, 28);
            lblIpServer.TabIndex = 10;
            lblIpServer.Text = "IP Server:";
            // 
            // lblPortServer
            // 
            lblPortServer.AutoSize = true;
            lblPortServer.Font = new Font("Segoe UI", 10F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lblPortServer.Location = new Point(310, 364);
            lblPortServer.Name = "lblPortServer";
            lblPortServer.Size = new Size(52, 28);
            lblPortServer.TabIndex = 11;
            lblPortServer.Text = "Port:";
            // 
            // lblTrangThaiKetNoi
            // 
            lblTrangThaiKetNoi.Font = new Font("Segoe UI", 10F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lblTrangThaiKetNoi.Location = new Point(64, 431);
            lblTrangThaiKetNoi.Name = "lblTrangThaiKetNoi";
            lblTrangThaiKetNoi.Size = new Size(459, 28);
            lblTrangThaiKetNoi.TabIndex = 12;
            lblTrangThaiKetNoi.Text = "Trạng thái: Chưa kết nối";
            lblTrangThaiKetNoi.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // txtIp
            // 
            txtIp.Font = new Font("Segoe UI", 11F);
            txtIp.Location = new Point(162, 360);
            txtIp.Name = "txtIp";
            txtIp.Size = new Size(131, 37);
            txtIp.TabIndex = 13;
            txtIp.Text = "127.0.0.1";
            txtIp.TextAlign = HorizontalAlignment.Center;
            // 
            // txtPort
            // 
            txtPort.Font = new Font("Segoe UI", 11F);
            txtPort.Location = new Point(368, 360);
            txtPort.Name = "txtPort";
            txtPort.Size = new Size(169, 37);
            txtPort.TabIndex = 14;
            txtPort.Text = "8888";
            txtPort.TextAlign = HorizontalAlignment.Center;
            // 
            // DangNhap
            // 
            AcceptButton = btnDangNhap;
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.White;
            CancelButton = btnThoat;
            ClientSize = new Size(600, 732);
            Controls.Add(txtPort);
            Controls.Add(txtIp);
            Controls.Add(lblTrangThaiKetNoi);
            Controls.Add(lblPortServer);
            Controls.Add(lblIpServer);
            Controls.Add(lblTieuDe);
            Controls.Add(lblPhuDe);
            Controls.Add(lblTenDangNhap);
            Controls.Add(txtTenDangNhap);
            Controls.Add(lblMatKhau);
            Controls.Add(txtMatKhau);
            Controls.Add(lblLoi);
            Controls.Add(btnDangNhap);
            Controls.Add(btnThoat);
            Controls.Add(llbDangKy);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Margin = new Padding(4, 5, 4, 5);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "DangNhap";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Đăng nhập";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label lblTieuDe;
        private Label lblPhuDe;
        private Label lblTenDangNhap;
        private TextBox txtTenDangNhap;
        private Label lblMatKhau;
        private TextBox txtMatKhau;
        private Label lblLoi;
        private Button btnDangNhap;
        private Button btnThoat;
        private LinkLabel llbDangKy;
        private Label lblIpServer;
        private Label lblPortServer;
        private Label lblTrangThaiKetNoi;
        private TextBox txtIp;
        private TextBox txtPort;
    }
}
