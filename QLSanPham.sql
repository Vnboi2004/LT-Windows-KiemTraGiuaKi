CREATE DATABASE QLSanPham

USE QLSanPham

CREATE TABLE LoaiSP
(
	MaLoai CHAR(2),
	TenLoai NVARCHAR(30),
	PRIMARY KEY (MaLoai)
);

CREATE TABLE SanPham 
(
	MaSP CHAR(6),
	TenSP NVARCHAR(30),
	NgayNhap DATETIME,
	MaLoai CHAR(2),
	PRIMARY KEY (MaSP),
	FOREIGN KEY (MaLoai) REFERENCES LoaiSP(MaLoai)
);


INSERT INTO LoaiSP
VALUES ('01', N'Bánh'),
	   ('02', N'Sữa'),
	   ('03', N'Nước ngọt');

INSERT INTO SanPham
VALUES ('A00001', N'Bánh quy', '2024-12-15', '01'),
	   ('A00002', N'Bánh ốc quế', '2024-12-15', '01'),
	   ('A00003', N'Sữa ông thọ', '2024-12-16', '02'),
	   ('A00004', N'Pepsi', '2024-12-17', '03');
