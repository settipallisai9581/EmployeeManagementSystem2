using Microsoft.EntityFrameworkCore;

namespace EmployeeManagementSystem2.Server.Data;

public static class DatabaseStartupInitializer
{
    public static async Task InitializeAsync(ApplicationDbContext dbContext, CancellationToken cancellationToken = default)
    {
        await dbContext.Database.EnsureCreatedAsync(cancellationToken);

        const string loginProcedureSql = """
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
        e.Id AS EmployeeId
    FROM dbo.Users u
    LEFT JOIN dbo.Employees e ON e.UserId = u.Id
    WHERE u.Email = @Email
      AND u.IsActive = 1;
END;
""";

        const string registerProcedureSql = """
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
        FROM dbo.Users u
        WHERE u.Username = @Username OR u.Email = @Email
    ) OR EXISTS (
        SELECT 1
        FROM dbo.Employees e
        WHERE e.Email = @Email
    )
    BEGIN
        SELECT
            CAST(0 AS INT) AS UserId,
            CAST(N'' AS NVARCHAR(50)) AS Username,
            CAST(N'' AS NVARCHAR(100)) AS Email,
            CAST(0 AS INT) AS EmployeeId
        WHERE 1 = 0;
        RETURN;
    END

    DECLARE @DepartmentName NVARCHAR(100) = (SELECT TOP 1 Name FROM dbo.Departments WHERE Id = @DepartmentId);
    DECLARE @RoleName NVARCHAR(100) = (SELECT TOP 1 Name FROM dbo.Roles WHERE Id = @RoleId);

    IF @DepartmentName IS NULL OR @RoleName IS NULL
    BEGIN
        SELECT
            CAST(0 AS INT) AS UserId,
            CAST(N'' AS NVARCHAR(50)) AS Username,
            CAST(N'' AS NVARCHAR(100)) AS Email,
            CAST(0 AS INT) AS EmployeeId
        WHERE 1 = 0;
        RETURN;
    END

    INSERT INTO dbo.Users (Username, Email, PasswordHash, CreatedDate, IsActive)
    VALUES (@Username, @Email, @PasswordHash, SYSUTCDATETIME(), 1);

    DECLARE @UserId INT = SCOPE_IDENTITY();

    INSERT INTO dbo.Employees
    (
        FirstName, LastName, Email, Phone, DateOfBirth, HireDate, Address, City, State, ZipCode,
        PhotoPath, Salary, IsActive, CreatedDate, UpdatedDate, DepartmentId, RoleId, UserId,
        Department, Position
    )
    VALUES
    (
        @FirstName, @LastName, @Email, NULL, NULL, SYSUTCDATETIME(), NULL, NULL, NULL, NULL,
        NULL, 1, 1, SYSUTCDATETIME(), NULL, @DepartmentId, @RoleId, @UserId,
        @DepartmentName, @RoleName
    );

    DECLARE @EmployeeId INT = SCOPE_IDENTITY();

    SELECT
        @UserId AS UserId,
        @Username AS Username,
        @Email AS Email,
        @EmployeeId AS EmployeeId;
END;
""";

        const string employeesGetAllProcedureSql = """
CREATE OR ALTER PROCEDURE dbo.usp_Employees_GetAll
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        e.Id,
        e.FirstName,
        e.LastName,
        e.Email,
        e.Phone,
        e.DateOfBirth,
        e.HireDate,
        e.Address,
        e.City,
        e.State,
        e.ZipCode,
        e.PhotoPath,
        e.Salary,
        e.IsActive,
        e.DepartmentId,
        d.Name AS DepartmentName,
        e.RoleId,
        r.Name AS RoleName,
        e.CreatedDate,
        e.UpdatedDate
    FROM dbo.Employees e
    LEFT JOIN dbo.Departments d ON d.Id = e.DepartmentId
    LEFT JOIN dbo.Roles r ON r.Id = e.RoleId
    ORDER BY e.LastName, e.FirstName;
END;
""";

        const string employeesGetByIdProcedureSql = """
CREATE OR ALTER PROCEDURE dbo.usp_Employees_GetById
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT TOP 1
        e.Id,
        e.FirstName,
        e.LastName,
        e.Email,
        e.Phone,
        e.DateOfBirth,
        e.HireDate,
        e.Address,
        e.City,
        e.State,
        e.ZipCode,
        e.PhotoPath,
        e.Salary,
        e.IsActive,
        e.DepartmentId,
        d.Name AS DepartmentName,
        e.RoleId,
        r.Name AS RoleName,
        e.CreatedDate,
        e.UpdatedDate
    FROM dbo.Employees e
    LEFT JOIN dbo.Departments d ON d.Id = e.DepartmentId
    LEFT JOIN dbo.Roles r ON r.Id = e.RoleId
    WHERE e.Id = @Id;
END;
""";

        const string employeesCreateProcedureSql = """
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
        NULL, CASE WHEN @Salary IS NULL OR @Salary < 1 THEN 1 ELSE @Salary END, 1, SYSUTCDATETIME(), NULL, @DepartmentId, @RoleId, @UserId,
        @DepartmentName, @RoleName
    );

    DECLARE @NewId INT = SCOPE_IDENTITY();
    EXEC dbo.usp_Employees_GetById @Id = @NewId;
END;
""";

        const string employeesUpdateProcedureSql = """
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
        Salary = CASE WHEN @Salary IS NULL OR @Salary < 1 THEN 1 ELSE @Salary END,
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
""";

        const string employeesDeleteProcedureSql = """
CREATE OR ALTER PROCEDURE dbo.usp_Employees_Delete
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;
    DELETE FROM dbo.Employees WHERE Id = @Id;
    SELECT @@ROWCOUNT AS RowsDeleted;
END;
""";

        await dbContext.Database.ExecuteSqlRawAsync(loginProcedureSql, cancellationToken);
        await dbContext.Database.ExecuteSqlRawAsync(registerProcedureSql, cancellationToken);
        await dbContext.Database.ExecuteSqlRawAsync(employeesGetAllProcedureSql, cancellationToken);
        await dbContext.Database.ExecuteSqlRawAsync(employeesGetByIdProcedureSql, cancellationToken);
        await dbContext.Database.ExecuteSqlRawAsync(employeesCreateProcedureSql, cancellationToken);
        await dbContext.Database.ExecuteSqlRawAsync(employeesUpdateProcedureSql, cancellationToken);
        await dbContext.Database.ExecuteSqlRawAsync(employeesDeleteProcedureSql, cancellationToken);
    }
}
