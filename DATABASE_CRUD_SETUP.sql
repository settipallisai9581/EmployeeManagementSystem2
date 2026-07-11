SET NOCOUNT ON;

/* 1) Core Tables */
IF OBJECT_ID(N'dbo.Users', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Users
    (
        Id INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Users PRIMARY KEY,
        Username NVARCHAR(50) NOT NULL,
        Email NVARCHAR(100) NOT NULL,
        PasswordHash NVARCHAR(MAX) NOT NULL,
        CreatedDate DATETIME2 NOT NULL CONSTRAINT DF_Users_CreatedDate DEFAULT SYSUTCDATETIME(),
        LastLoginDate DATETIME2 NULL,
        IsActive BIT NOT NULL CONSTRAINT DF_Users_IsActive DEFAULT (1)
    );
END;

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Users_Username' AND object_id = OBJECT_ID(N'dbo.Users'))
    CREATE UNIQUE INDEX IX_Users_Username ON dbo.Users(Username);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Users_Email' AND object_id = OBJECT_ID(N'dbo.Users'))
    CREATE UNIQUE INDEX IX_Users_Email ON dbo.Users(Email);

IF OBJECT_ID(N'dbo.Departments', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Departments
    (
        Id INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Departments PRIMARY KEY,
        Name NVARCHAR(100) NOT NULL,
        Description NVARCHAR(MAX) NULL,
        CreatedDate DATETIME2 NOT NULL CONSTRAINT DF_Departments_CreatedDate DEFAULT SYSUTCDATETIME(),
        UpdatedDate DATETIME2 NULL,
        IsActive BIT NOT NULL CONSTRAINT DF_Departments_IsActive DEFAULT (1)
    );
END;

IF OBJECT_ID(N'dbo.Roles', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Roles
    (
        Id INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Roles PRIMARY KEY,
        Name NVARCHAR(100) NOT NULL,
        Description NVARCHAR(MAX) NULL,
        CreatedDate DATETIME2 NOT NULL CONSTRAINT DF_Roles_CreatedDate DEFAULT SYSUTCDATETIME(),
        UpdatedDate DATETIME2 NULL,
        IsActive BIT NOT NULL CONSTRAINT DF_Roles_IsActive DEFAULT (1)
    );
END;

IF OBJECT_ID(N'dbo.Employees', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Employees
    (
        Id INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Employees PRIMARY KEY,
        FirstName NVARCHAR(100) NOT NULL,
        LastName NVARCHAR(100) NOT NULL,
        Email NVARCHAR(100) NOT NULL,
        Department NVARCHAR(100) NOT NULL,
        Position NVARCHAR(100) NOT NULL,
        Salary DECIMAL(18,2) NOT NULL CONSTRAINT DF_Employees_Salary DEFAULT (0),
        HireDate DATETIME2 NOT NULL CONSTRAINT DF_Employees_HireDate DEFAULT SYSUTCDATETIME(),
        IsActive BIT NOT NULL CONSTRAINT DF_Employees_IsActive DEFAULT (1),
        Phone NVARCHAR(MAX) NULL,
        DateOfBirth DATETIME2 NULL,
        Address NVARCHAR(MAX) NULL,
        City NVARCHAR(MAX) NULL,
        State NVARCHAR(MAX) NULL,
        ZipCode NVARCHAR(MAX) NULL,
        PhotoPath NVARCHAR(MAX) NULL,
        CreatedDate DATETIME2 NOT NULL CONSTRAINT DF_Employees_CreatedDate DEFAULT SYSUTCDATETIME(),
        UpdatedDate DATETIME2 NULL,
        DepartmentId INT NULL,
        RoleId INT NULL,
        UserId INT NULL
    );
END;

IF COL_LENGTH('dbo.Employees','Department') IS NULL
    ALTER TABLE dbo.Employees ADD Department NVARCHAR(100) NOT NULL CONSTRAINT DF_Employees_Department DEFAULT (N'Unknown');
IF COL_LENGTH('dbo.Employees','Position') IS NULL
    ALTER TABLE dbo.Employees ADD Position NVARCHAR(100) NOT NULL CONSTRAINT DF_Employees_Position DEFAULT (N'Employee');
IF COL_LENGTH('dbo.Employees','Salary') IS NULL
    ALTER TABLE dbo.Employees ADD Salary DECIMAL(18,2) NOT NULL CONSTRAINT DF_Employees_Salary_Compat DEFAULT (0);

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Departments_Name' AND object_id = OBJECT_ID(N'dbo.Departments'))
    CREATE UNIQUE INDEX IX_Departments_Name ON dbo.Departments(Name);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Roles_Name' AND object_id = OBJECT_ID(N'dbo.Roles'))
    CREATE UNIQUE INDEX IX_Roles_Name ON dbo.Roles(Name);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Employees_Email' AND object_id = OBJECT_ID(N'dbo.Employees'))
    CREATE UNIQUE INDEX IX_Employees_Email ON dbo.Employees(Email);

IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_Employees_Departments_DepartmentId')
   AND OBJECT_ID(N'dbo.Departments', N'U') IS NOT NULL
BEGIN
    ALTER TABLE dbo.Employees WITH NOCHECK ADD CONSTRAINT FK_Employees_Departments_DepartmentId
    FOREIGN KEY (DepartmentId) REFERENCES dbo.Departments(Id);
END;

IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_Employees_Roles_RoleId')
   AND OBJECT_ID(N'dbo.Roles', N'U') IS NOT NULL
BEGIN
    ALTER TABLE dbo.Employees WITH NOCHECK ADD CONSTRAINT FK_Employees_Roles_RoleId
    FOREIGN KEY (RoleId) REFERENCES dbo.Roles(Id);
END;

IF OBJECT_ID(N'dbo.Users', N'U') IS NOT NULL
   AND NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_Employees_Users_UserId')
BEGIN
    ALTER TABLE dbo.Employees WITH NOCHECK ADD CONSTRAINT FK_Employees_Users_UserId
    FOREIGN KEY (UserId) REFERENCES dbo.Users(Id);
END;

IF NOT EXISTS (SELECT 1 FROM dbo.Departments)
BEGIN
    INSERT INTO dbo.Departments (Name, Description, CreatedDate, IsActive)
    VALUES (N'IT', N'Information Technology', SYSUTCDATETIME(), 1),
           (N'HR', N'Human Resources', SYSUTCDATETIME(), 1),
           (N'Finance', N'Finance Department', SYSUTCDATETIME(), 1),
           (N'Sales', N'Sales Department', SYSUTCDATETIME(), 1);
END;

IF NOT EXISTS (SELECT 1 FROM dbo.Roles)
BEGIN
    INSERT INTO dbo.Roles (Name, Description, CreatedDate, IsActive)
    VALUES (N'Admin', N'System Administrator', SYSUTCDATETIME(), 1),
           (N'Manager', N'Department Manager', SYSUTCDATETIME(), 1),
           (N'Employee', N'Regular Employee', SYSUTCDATETIME(), 1),
           (N'Intern', N'Intern Employee', SYSUTCDATETIME(), 1);
END;
GO

SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO

/* 2) Auth SPs */
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

    IF EXISTS (SELECT 1 FROM dbo.Users WHERE Username = @Username OR Email = @Email)
    BEGIN
        SELECT CAST(0 AS INT) AS UserId, CAST(N'' AS NVARCHAR(50)) AS Username, CAST(N'' AS NVARCHAR(100)) AS Email, CAST(0 AS INT) AS EmployeeId
        WHERE 1 = 0;
        RETURN;
    END;

    INSERT INTO dbo.Users (Username, Email, PasswordHash, CreatedDate, IsActive)
    VALUES (@Username, @Email, @PasswordHash, SYSUTCDATETIME(), 1);

    DECLARE @UserId INT = SCOPE_IDENTITY();

    SELECT @UserId AS UserId, @Username AS Username, @Email AS Email, CAST(0 AS INT) AS EmployeeId;
END;
GO

/* 3) Employee CRUD SPs */
CREATE OR ALTER PROCEDURE dbo.usp_Employees_GetAll
AS
BEGIN
    SET NOCOUNT ON;
    SELECT e.Id, e.FirstName, e.LastName, e.Email, e.Phone, e.DateOfBirth, e.HireDate,
           e.Address, e.City, e.State, e.ZipCode, e.PhotoPath, e.Salary, e.IsActive,
           e.DepartmentId, d.Name AS DepartmentName, e.RoleId, r.Name AS RoleName,
           e.CreatedDate, e.UpdatedDate
    FROM dbo.Employees e
    LEFT JOIN dbo.Departments d ON d.Id = e.DepartmentId
    LEFT JOIN dbo.Roles r ON r.Id = e.RoleId
    ORDER BY e.LastName, e.FirstName;
END;
GO

CREATE OR ALTER PROCEDURE dbo.usp_Employees_GetById
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT TOP 1 e.Id, e.FirstName, e.LastName, e.Email, e.Phone, e.DateOfBirth, e.HireDate,
           e.Address, e.City, e.State, e.ZipCode, e.PhotoPath, e.Salary, e.IsActive,
           e.DepartmentId, d.Name AS DepartmentName, e.RoleId, r.Name AS RoleName,
           e.CreatedDate, e.UpdatedDate
    FROM dbo.Employees e
    LEFT JOIN dbo.Departments d ON d.Id = e.DepartmentId
    LEFT JOIN dbo.Roles r ON r.Id = e.RoleId
    WHERE e.Id = @Id;
END;
GO

CREATE OR ALTER PROCEDURE dbo.usp_Employees_Create
    @FirstName NVARCHAR(100),
    @LastName NVARCHAR(100),
    @Email NVARCHAR(100),
    @Phone NVARCHAR(MAX) = NULL,
    @DateOfBirth DATETIME2 = NULL,
    @HireDate DATETIME2,
    @Address NVARCHAR(MAX) = NULL,
    @City NVARCHAR(MAX) = NULL,
    @State NVARCHAR(MAX) = NULL,
    @ZipCode NVARCHAR(MAX) = NULL,
    @Salary DECIMAL(18,2) = NULL,
    @DepartmentId INT,
    @RoleId INT,
    @UserId INT = NULL
AS
BEGIN
    SET NOCOUNT ON;

    IF EXISTS (SELECT 1 FROM dbo.Employees WHERE Email = @Email)
        THROW 51000, 'Employee email already exists.', 1;

    DECLARE @DepartmentName NVARCHAR(100) = (SELECT TOP 1 Name FROM dbo.Departments WHERE Id = @DepartmentId);
    DECLARE @RoleName NVARCHAR(100) = (SELECT TOP 1 Name FROM dbo.Roles WHERE Id = @RoleId);

    IF @DepartmentName IS NULL OR @RoleName IS NULL
        THROW 51001, 'Invalid DepartmentId or RoleId.', 1;

    INSERT INTO dbo.Employees
    (
        FirstName, LastName, Email, Phone, DateOfBirth, HireDate, Address, City, State, ZipCode,
        PhotoPath, Salary, IsActive, CreatedDate, UpdatedDate, DepartmentId, RoleId, UserId,
        Department, Position
    )
    VALUES
    (
        @FirstName, @LastName, @Email, @Phone, @DateOfBirth, @HireDate, @Address, @City, @State, @ZipCode,
        NULL, ISNULL(@Salary, 0), 1, SYSUTCDATETIME(), NULL, @DepartmentId, @RoleId, @UserId,
        @DepartmentName, @RoleName
    );

    DECLARE @NewId INT = SCOPE_IDENTITY();
    EXEC dbo.usp_Employees_GetById @Id = @NewId;
END;
GO

CREATE OR ALTER PROCEDURE dbo.usp_Employees_Update
    @Id INT,
    @FirstName NVARCHAR(100),
    @LastName NVARCHAR(100),
    @Email NVARCHAR(100),
    @Phone NVARCHAR(MAX) = NULL,
    @DateOfBirth DATETIME2 = NULL,
    @HireDate DATETIME2,
    @Address NVARCHAR(MAX) = NULL,
    @City NVARCHAR(MAX) = NULL,
    @State NVARCHAR(MAX) = NULL,
    @ZipCode NVARCHAR(MAX) = NULL,
    @Salary DECIMAL(18,2) = NULL,
    @IsActive BIT,
    @DepartmentId INT,
    @RoleId INT,
    @UserId INT = NULL
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS (SELECT 1 FROM dbo.Employees WHERE Id = @Id)
        THROW 51002, 'Employee not found.', 1;

    IF EXISTS (SELECT 1 FROM dbo.Employees WHERE Email = @Email AND Id <> @Id)
        THROW 51003, 'Employee email already exists.', 1;

    DECLARE @DepartmentName NVARCHAR(100) = (SELECT TOP 1 Name FROM dbo.Departments WHERE Id = @DepartmentId);
    DECLARE @RoleName NVARCHAR(100) = (SELECT TOP 1 Name FROM dbo.Roles WHERE Id = @RoleId);

    IF @DepartmentName IS NULL OR @RoleName IS NULL
        THROW 51004, 'Invalid DepartmentId or RoleId.', 1;

    UPDATE dbo.Employees
    SET FirstName = @FirstName,
        LastName = @LastName,
        Email = @Email,
        Phone = @Phone,
        DateOfBirth = @DateOfBirth,
        HireDate = @HireDate,
        Address = @Address,
        City = @City,
        State = @State,
        ZipCode = @ZipCode,
        Salary = ISNULL(@Salary, 0),
        IsActive = @IsActive,
        DepartmentId = @DepartmentId,
        RoleId = @RoleId,
        UserId = @UserId,
        Department = @DepartmentName,
        Position = @RoleName,
        UpdatedDate = SYSUTCDATETIME()
    WHERE Id = @Id;

    EXEC dbo.usp_Employees_GetById @Id = @Id;
END;
GO

CREATE OR ALTER PROCEDURE dbo.usp_Employees_Delete
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;
    DELETE FROM dbo.Employees WHERE Id = @Id;
    SELECT @@ROWCOUNT AS RowsDeleted;
END;
GO
