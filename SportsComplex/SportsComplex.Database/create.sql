CREATE DATABASE SportsComplex;
GO
USE [SportsComplex];

CREATE TABLE tblClassType (
	Id INT NOT NULL IDENTITY(1,1),
	Name NVARCHAR(100) NOT NULL,
	CONSTRAINT PK_tblClassType_Id PRIMARY KEY (Id),
	CONSTRAINT UQ_tblClassType_Name UNIQUE (Name)
);
GO

CREATE TABLE tblClass (
	Id INT NOT NULL IDENTITY(1, 1),
	ClassTypeId INT NOT NULL,
	Area INT NOT NULL,
	Rate NUMERIC(18, 4) NOT NULL
	CONSTRAINT PK_tblClass_Id PRIMARY KEY (Id),
	CONSTRAINT FK_tblClass_ClassTypeId_tblClassType_Id FOREIGN KEY (ClassTypeId) REFERENCES tblClassType (Id)
);
GO

CREATE TABLE tblRenter (
	Id INT NOT NULL IDENTITY(1, 1),
	FirstName NVARCHAR(50) NOT NULL,
	LastName NVARCHAR(50) NOT NULL,
	Phone NVARCHAR(30) NOT NULL,
	CONSTRAINT PK_tblRenter_Id PRIMARY KEY (Id),
	CONSTRAINT UQ_tblRenter_Phone UNIQUE (Phone)
);
GO

CREATE TABLE tblRent (
	Id INT NOT NULL IDENTITY(1, 1),
	RenterId INT NOT NULL,
	ClassId INT NOT NULL,
	DateStart DATETIME NOT NULL,
	DateEnd DATETIME NOT NULL,
	Cost NUMERIC(18, 4) NOT NULL,
	Deleted BIT NOT NULL CONSTRAINT [DF_tblRent_Deleted] DEFAULT 0,
	CONSTRAINT PK_tblRent_Id PRIMARY KEY (Id),
	CONSTRAINT FK_tblRent_RenterId_tblRenter_Id FOREIGN KEY (RenterId) REFERENCES tblRenter (Id),
	CONSTRAINT FK_tblRent_ClassId_tblClass_Id FOREIGN KEY (ClassId) REFERENCES tblClass (Id)
);

CREATE TABLE tblUser (
	Id INT NOT NULL IDENTITY(1,1),
	FirstName NVARCHAR(50) NOT NULL,
	LastName NVARCHAR(50) NOT NULL,
	[Login] VARCHAR(50) NOT NULL,
	[Password] VARCHAR(50) NOT NULL,
	[Disabled] BIT NOT NULL,
	--[Role] INT NOT NULL,
	CONSTRAINT PK_tblUser_Id PRIMARY KEY (Id),
	CONSTRAINT UQ_tblUser_Login UNIQUE ([Login])
);
GO