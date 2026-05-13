-- SQL script to create database and tables for User Registration Application
-- Safe to re-run in SQL Server Management Studio (idempotent).

IF DB_ID(N'UserRegistrationDb') IS NULL
BEGIN
    CREATE DATABASE [UserRegistrationDb];
END
GO

USE [UserRegistrationDb];
GO

-- States table (for autocomplete dropdown)
IF OBJECT_ID(N'dbo.States', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[States]
    (
        [Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        [Name] NVARCHAR(100) NOT NULL
    );
END
GO

-- Users table
IF OBJECT_ID(N'dbo.Users', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[Users]
    (
        [Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        [Name] NVARCHAR(100) NOT NULL,
        [Email] NVARCHAR(256) NOT NULL,
        [Mobile] NVARCHAR(10) NOT NULL,
        [Address] NVARCHAR(500) NOT NULL,
        [Gender] NVARCHAR(10) NOT NULL,
        [StateId] INT NOT NULL,
        [Hobbies] NVARCHAR(500) NOT NULL,
        [CreatedAt] DATETIME NOT NULL DEFAULT(GETDATE()),
        CONSTRAINT FK_Users_States FOREIGN KEY([StateId]) REFERENCES [dbo].[States]([Id])
    );
END
GO

-- Seed sample states (only if empty)
IF NOT EXISTS (SELECT 1 FROM [dbo].[States])
BEGIN
    INSERT INTO [dbo].[States] ([Name]) VALUES
    ('Alabama'),
    ('Alaska'),
    ('Arizona'),
    ('Arkansas'),
    ('California'),
    ('Colorado'),
    ('Connecticut'),
    ('Delaware'),
    ('Florida'),
    ('Georgia'),
    ('Hawaii'),
    ('Idaho'),
    ('Illinois'),
    ('Indiana'),
    ('Iowa'),
    ('Kansas'),
    ('Kentucky'),
    ('Louisiana'),
    ('Maine'),
    ('Maryland'),
    ('Massachusetts'),
    ('Michigan'),
    ('Minnesota'),
    ('Mississippi'),
    ('Missouri'),
    ('Montana'),
    ('Nebraska'),
    ('Nevada'),
    ('New Hampshire'),
    ('New Jersey'),
    ('New Mexico'),
    ('New York'),
    ('North Carolina'),
    ('North Dakota'),
    ('Ohio'),
    ('Oklahoma'),
    ('Oregon'),
    ('Pennsylvania'),
    ('Rhode Island'),
    ('South Carolina'),
    ('South Dakota'),
    ('Tennessee'),
    ('Texas'),
    ('Utah'),
    ('Vermont'),
    ('Virginia'),
    ('Washington'),
    ('West Virginia'),
    ('Wisconsin'),
    ('Wyoming');
END
GO

-- Quick check selects
SELECT TOP 10 * FROM [dbo].[States];
SELECT TOP 10 * FROM [dbo].[Users];
GO
