USE [master];
GO
IF DB_ID(N'MinorProject5_GameLibrary') IS NULL
    EXEC(N'CREATE DATABASE [MinorProject5_GameLibrary]');
GO
USE [MinorProject5_GameLibrary];
GO
IF OBJECT_ID(N'dbo.Franchises', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Franchises (
        Id int IDENTITY(1,1) NOT NULL CONSTRAINT PK_Franchises PRIMARY KEY,
        Name nvarchar(100) NOT NULL CONSTRAINT UQ_Franchises_Name UNIQUE,
        Developer nvarchar(120) NOT NULL
    );
END;
GO
IF OBJECT_ID(N'dbo.Platforms', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Platforms (
        Id int IDENTITY(1,1) NOT NULL CONSTRAINT PK_Platforms PRIMARY KEY,
        Name nvarchar(80) NOT NULL CONSTRAINT UQ_Platforms_Name UNIQUE,
        Manufacturer nvarchar(80) NOT NULL
    );
END;
GO
IF OBJECT_ID(N'dbo.Games', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Games (
        Id int IDENTITY(1,1) NOT NULL CONSTRAINT PK_Games PRIMARY KEY,
        Title nvarchar(120) NOT NULL,
        Notes nvarchar(1000) NOT NULL CONSTRAINT DF_Games_Notes DEFAULT N'',
        ReleaseYear int NOT NULL CONSTRAINT CK_Games_ReleaseYear CHECK (ReleaseYear BETWEEN 1970 AND 2100),
        FranchiseId int NOT NULL CONSTRAINT FK_Games_Franchises REFERENCES dbo.Franchises(Id),
        PlatformId int NOT NULL CONSTRAINT FK_Games_Platforms REFERENCES dbo.Platforms(Id)
    );
    CREATE INDEX IX_Games_FranchiseId ON dbo.Games(FranchiseId);
    CREATE INDEX IX_Games_PlatformId ON dbo.Games(PlatformId);
END;
GO
