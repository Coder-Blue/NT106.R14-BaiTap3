-- Chạy script này một lần trên SQL Server/LocalDB trước khi chạy server.

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
    Email           NVARCHAR(100) NOT NULL,
    NgayTao         DATETIME2     NOT NULL DEFAULT SYSDATETIME(),
    LanDangNhapCuoi DATETIME2     NULL
);
GO
