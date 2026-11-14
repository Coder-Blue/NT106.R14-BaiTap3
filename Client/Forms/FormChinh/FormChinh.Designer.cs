namespace NT106.R14.BaiTap3.Client.Forms
{
    partial class FormChinh
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
            lblChaoMung = new Label();
            grpThongTin = new GroupBox();
            lblTenDangNhapLabel = new Label();
            lblTenDangNhap = new Label();
            lblHoTenLabel = new Label();
            lblHoTen = new Label();
            lblEmailLabel = new Label();
            lblEmail = new Label();
            lblNgaySinhLabel = new Label();
            lblNgaySinh = new Label();
            lblUserIdLabel = new Label();
            lblUserId = new Label();
            lblTrangThai = new Label();
            btnDangXuat = new Button();
            grpThongTin.SuspendLayout();
            SuspendLayout();
            //
            //lblTieuDe
            //
            lblTieuDe.Font = new Font("Segoe UI", 22F, FontStyle.Bold);
            lblTieuDe.ForeColor = Color.FromArgb(35, 78, 120);
            lblTieuDe.Location = new Point(0, 24);
            lblTieuDe.Name = "lblTieuDe";
            lblTieuDe.Size = new Size(640, 44);
            lblTieuDe.TabIndex = 0;
            lblTieuDe.Text = "TRANG CHÍNH";
            lblTieuDe.TextAlign = ContentAlignment.MiddleCenter;
            //
            //lblPhuDe
            //
            lblPhuDe.Font = new Font("Segoe UI", 10.5F);
            lblPhuDe.ForeColor = Color.FromArgb(100, 100, 100);
            lblPhuDe.Location = new Point(0, 72);
            lblPhuDe.Name = "lblPhuDe";
            lblPhuDe.Size = new Size(640, 24);
            lblPhuDe.TabIndex = 1;
            lblPhuDe.Text = "Thông tin người dùng nhận từ server";
            lblPhuDe.TextAlign = ContentAlignment.MiddleCenter;
            //
            //lblChaoMung
            //
            lblChaoMung.AutoEllipsis = true;
            lblChaoMung.Font = new Font("Segoe UI", 13F, FontStyle.Bold);
            lblChaoMung.ForeColor = Color.FromArgb(35, 78, 120);
            lblChaoMung.Location = new Point(48, 106);
            lblChaoMung.Name = "lblChaoMung";
            lblChaoMung.Size = new Size(544, 28);
            lblChaoMung.TabIndex = 2;
            lblChaoMung.Text = "Xin chào!";
            lblChaoMung.TextAlign = ContentAlignment.MiddleLeft;
            lblChaoMung.UseMnemonic = false;
            //
            //grpThongTin
            //
            grpThongTin.Controls.Add(lblTenDangNhapLabel);
            grpThongTin.Controls.Add(lblTenDangNhap);
            grpThongTin.Controls.Add(lblHoTenLabel);
            grpThongTin.Controls.Add(lblHoTen);
            grpThongTin.Controls.Add(lblEmailLabel);
            grpThongTin.Controls.Add(lblEmail);
            grpThongTin.Controls.Add(lblNgaySinhLabel);
            grpThongTin.Controls.Add(lblNgaySinh);
            grpThongTin.Controls.Add(lblUserIdLabel);
            grpThongTin.Controls.Add(lblUserId);
            grpThongTin.Font = new Font("Segoe UI", 10F);
            grpThongTin.ForeColor = Color.FromArgb(50, 50, 50);
            grpThongTin.Location = new Point(48, 146);
            grpThongTin.Name = "grpThongTin";
            grpThongTin.Size = new Size(544, 224);
            grpThongTin.TabIndex = 3;
            grpThongTin.TabStop = false;
            grpThongTin.Text = "Thông tin tài khoản";
            //
            //lblTenDangNhapLabel
            //
            lblTenDangNhapLabel.AutoSize = true;
            lblTenDangNhapLabel.ForeColor = Color.FromArgb(110, 110, 110);
            lblTenDangNhapLabel.Location = new Point(24, 36);
            lblTenDangNhapLabel.Name = "lblTenDangNhapLabel";
            lblTenDangNhapLabel.TabIndex = 0;
            lblTenDangNhapLabel.Text = "Tên đăng nhập:";
            //
            //lblTenDangNhap
            //
            lblTenDangNhap.AutoEllipsis = true;
            lblTenDangNhap.AutoSize = false;
            lblTenDangNhap.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            lblTenDangNhap.Location = new Point(200, 36);
            lblTenDangNhap.Name = "lblTenDangNhap";
            lblTenDangNhap.Size = new Size(320, 23);
            lblTenDangNhap.TabIndex = 1;
            lblTenDangNhap.UseMnemonic = false;
            //
            //lblHoTenLabel
            //
            lblHoTenLabel.AutoSize = true;
            lblHoTenLabel.ForeColor = Color.FromArgb(110, 110, 110);
            lblHoTenLabel.Location = new Point(24, 72);
            lblHoTenLabel.Name = "lblHoTenLabel";
            lblHoTenLabel.TabIndex = 2;
            lblHoTenLabel.Text = "Họ tên:";
            //
            //lblHoTen
            //
            lblHoTen.AutoEllipsis = true;
            lblHoTen.AutoSize = false;
            lblHoTen.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            lblHoTen.Location = new Point(200, 72);
            lblHoTen.Name = "lblHoTen";
            lblHoTen.Size = new Size(320, 23);
            lblHoTen.TabIndex = 3;
            lblHoTen.UseMnemonic = false;
            //
            //lblEmailLabel
            //
            lblEmailLabel.AutoSize = true;
            lblEmailLabel.ForeColor = Color.FromArgb(110, 110, 110);
            lblEmailLabel.Location = new Point(24, 108);
            lblEmailLabel.Name = "lblEmailLabel";
            lblEmailLabel.TabIndex = 4;
            lblEmailLabel.Text = "Email:";
            //
            //lblEmail
            //
            lblEmail.AutoEllipsis = true;
            lblEmail.AutoSize = false;
            lblEmail.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            lblEmail.Location = new Point(200, 108);
            lblEmail.Name = "lblEmail";
            lblEmail.Size = new Size(320, 23);
            lblEmail.TabIndex = 5;
            lblEmail.UseMnemonic = false;
            //
            //lblNgaySinhLabel
            //
            lblNgaySinhLabel.AutoSize = true;
            lblNgaySinhLabel.ForeColor = Color.FromArgb(110, 110, 110);
            lblNgaySinhLabel.Location = new Point(24, 144);
            lblNgaySinhLabel.Name = "lblNgaySinhLabel";
            lblNgaySinhLabel.TabIndex = 6;
            lblNgaySinhLabel.Text = "Ngày sinh:";
            //
            //lblNgaySinh
            //
            lblNgaySinh.AutoEllipsis = true;
            lblNgaySinh.AutoSize = false;
            lblNgaySinh.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            lblNgaySinh.Location = new Point(200, 144);
            lblNgaySinh.Name = "lblNgaySinh";
            lblNgaySinh.Size = new Size(320, 23);
            lblNgaySinh.TabIndex = 7;
            lblNgaySinh.UseMnemonic = false;
            //
            //lblUserIdLabel
            //
            lblUserIdLabel.AutoSize = true;
            lblUserIdLabel.ForeColor = Color.FromArgb(110, 110, 110);
            lblUserIdLabel.Location = new Point(24, 180);
            lblUserIdLabel.Name = "lblUserIdLabel";
            lblUserIdLabel.TabIndex = 8;
            lblUserIdLabel.Text = "Mã người dùng:";
            //
            //lblUserId
            //
            lblUserId.AutoEllipsis = true;
            lblUserId.AutoSize = false;
            lblUserId.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            lblUserId.Location = new Point(200, 180);
            lblUserId.Name = "lblUserId";
            lblUserId.Size = new Size(320, 23);
            lblUserId.TabIndex = 9;
            lblUserId.UseMnemonic = false;
            //
            //lblTrangThai
            //
            lblTrangThai.AutoEllipsis = true;
            lblTrangThai.Font = new Font("Segoe UI", 9F, FontStyle.Italic);
            lblTrangThai.ForeColor = Color.FromArgb(100, 100, 100);
            lblTrangThai.Location = new Point(48, 382);
            lblTrangThai.Name = "lblTrangThai";
            lblTrangThai.Size = new Size(544, 22);
            lblTrangThai.TabIndex = 4;
            lblTrangThai.Text = "Trạng thái: Đã kết nối";
            //
            //btnDangXuat
            //
            btnDangXuat.BackColor = Color.FromArgb(200, 70, 70);
            btnDangXuat.Cursor = Cursors.Hand;
            btnDangXuat.FlatAppearance.BorderSize = 0;
            btnDangXuat.FlatAppearance.MouseDownBackColor = Color.FromArgb(170, 50, 50);
            btnDangXuat.FlatAppearance.MouseOverBackColor = Color.FromArgb(220, 90, 90);
            btnDangXuat.FlatStyle = FlatStyle.Flat;
            btnDangXuat.Font = new Font("Segoe UI", 10.5F, FontStyle.Bold);
            btnDangXuat.ForeColor = Color.White;
            btnDangXuat.Location = new Point(220, 414);
            btnDangXuat.Name = "btnDangXuat";
            btnDangXuat.Size = new Size(200, 46);
            btnDangXuat.TabIndex = 5;
            btnDangXuat.Text = "Đăng xuất";
            btnDangXuat.UseVisualStyleBackColor = false;
            btnDangXuat.Click += btnDangXuat_Click;
            //
            //FormChinh
            //
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.White;
            ClientSize = new Size(640, 490);
            Controls.Add(lblTieuDe);
            Controls.Add(lblPhuDe);
            Controls.Add(lblChaoMung);
            Controls.Add(grpThongTin);
            Controls.Add(lblTrangThai);
            Controls.Add(btnDangXuat);
            Font = new Font("Segoe UI", 9F);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "FormChinh";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Trang chính - Bài tập 3";
            grpThongTin.ResumeLayout(false);
            grpThongTin.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label lblTieuDe;
        private Label lblPhuDe;
        private Label lblChaoMung;
        private GroupBox grpThongTin;
        private Label lblTenDangNhapLabel;
        private Label lblTenDangNhap;
        private Label lblHoTenLabel;
        private Label lblHoTen;
        private Label lblEmailLabel;
        private Label lblEmail;
        private Label lblNgaySinhLabel;
        private Label lblNgaySinh;
        private Label lblUserIdLabel;
        private Label lblUserId;
        private Label lblTrangThai;
        private Button btnDangXuat;
    }
}
