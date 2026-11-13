-- CSDL cho ung dung Quan ly nguoi dung (BaiTap2.2)
-- Chay script nay trong SQL Server Management Studio.

IF DB_ID(N'QuanLyNguoiDung') IS NULL
    CREATE DATABASE QuanLyNguoiDung;
GO

USE QuanLyNguoiDung;
GO

IF OBJECT_ID(N'dbo.Users', N'U') IS NULL
CREATE TABLE dbo.Users (
    UserId          INT IDENTITY(1,1) PRIMARY KEY,
    TenDangNhap     NVARCHAR(20)  NOT NULL UNIQUE,
    MatKhauBam      NVARCHAR(64)  NOT NULL,
    Salt            NVARCHAR(32)  NOT NULL,
    HoTen           NVARCHAR(50)  NOT NULL,
    Email           NVARCHAR(100) NULL,
    NgayTao         DATETIME2     NOT NULL DEFAULT SYSDATETIME(),
    LanDangNhapCuoi DATETIME2     NULL
);
GO
