IF DB_ID(N'QuanLyNguoiDung') IS NULL CREATE DATABASE QuanLyNguoiDung;
GO
USE QuanLyNguoiDung;
GO
IF OBJECT_ID(N'dbo.NguoiDung') IS NULL
CREATE TABLE dbo.NguoiDung (
    UserId       INT IDENTITY(1,1) PRIMARY KEY,
    Username     NVARCHAR(20)  NOT NULL CONSTRAINT UQ_NguoiDung_Username UNIQUE,
    PasswordHash NVARCHAR(64)  NOT NULL,
    PasswordSalt NVARCHAR(32)  NOT NULL,
    Email        NVARCHAR(100) NULL,
    FullName     NVARCHAR(50)  NOT NULL,
    Birthday     DATE          NULL
);
