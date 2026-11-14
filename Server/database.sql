-- Chạy script này trong SQL Server Management Studio (SSMS):
-- 1. Kết nối tới (localdb)\MSSQLLocalDB hoặc instance SQL Server của bạn.
-- 2. Mở file này và bấm Execute (F5).
-- 3. Script có thể chạy lại nhiều lần, không xóa dữ liệu đã có.

USE master;
GO
SET NOCOUNT ON;
GO

IF DB_ID(N'QuanLyNguoiDung') IS NULL
BEGIN
    CREATE DATABASE QuanLyNguoiDung;
    PRINT N'Đã tạo database QuanLyNguoiDung.';
END
ELSE
BEGIN
    PRINT N'Database QuanLyNguoiDung đã tồn tại.';
END
GO

USE QuanLyNguoiDung;
GO

IF OBJECT_ID(N'dbo.NguoiDung', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.NguoiDung (
        UserId       INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_NguoiDung PRIMARY KEY,
        Username     NVARCHAR(20)  NOT NULL CONSTRAINT UQ_NguoiDung_Username UNIQUE,
        PasswordHash NVARCHAR(64)  NOT NULL,
        PasswordSalt NVARCHAR(32)  NOT NULL,
        Email        NVARCHAR(100) NOT NULL,
        FullName     NVARCHAR(50)  NOT NULL,
        Birthday     DATE          NOT NULL
    );
    PRINT N'Đã tạo bảng dbo.NguoiDung.';
END
ELSE
BEGIN
    PRINT N'Bảng dbo.NguoiDung đã tồn tại.';
END
GO

SELECT COLUMN_NAME, DATA_TYPE, CHARACTER_MAXIMUM_LENGTH AS MaxLength, IS_NULLABLE
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_SCHEMA = N'dbo' AND TABLE_NAME = N'NguoiDung'
ORDER BY ORDINAL_POSITION;
GO
