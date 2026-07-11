/*
  Auth stored procedures for EmployeeDb
  - dbo.usp_Auth_LoginByEmail
  - dbo.usp_Auth_RegisterUser
*/

USE [EmployeeDb];
GO

CREATE OR ALTER PROCEDURE dbo.usp_Auth_LoginByEmail
    @Email NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT TOP (1)
        u.Id AS UserId,
        u.Username,
        u.Email,
        u.PasswordHash,
        CAST(NULL AS INT) AS EmployeeId
    FROM dbo.Users u
    WHERE u.Email = @Email
      AND u.IsActive = 1;
END;
GO

CREATE OR ALTER PROCEDURE dbo.usp_Auth_RegisterUser
    @Username NVARCHAR(50),
    @Email NVARCHAR(100),
    @PasswordHash NVARCHAR(MAX),
    @FirstName NVARCHAR(100),
    @LastName NVARCHAR(100),
    @DepartmentId INT,
    @RoleId INT
AS
BEGIN
    SET NOCOUNT ON;

    IF EXISTS (
        SELECT 1
        FROM dbo.Users
        WHERE Username = @Username OR Email = @Email
    )
    BEGIN
        SELECT
            CAST(0 AS INT) AS UserId,
            CAST(N'' AS NVARCHAR(50)) AS Username,
            CAST(N'' AS NVARCHAR(100)) AS Email,
            CAST(0 AS INT) AS EmployeeId
        WHERE 1 = 0;
        RETURN;
    END;

    INSERT INTO dbo.Users (Username, Email, PasswordHash, CreatedDate, IsActive)
    VALUES (@Username, @Email, @PasswordHash, SYSUTCDATETIME(), 1);

    DECLARE @UserId INT = SCOPE_IDENTITY();

    SELECT
        @UserId AS UserId,
        @Username AS Username,
        @Email AS Email,
        CAST(0 AS INT) AS EmployeeId;
END;
GO
