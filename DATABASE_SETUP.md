# Database Setup Instructions

## Prerequisites
- .NET 8 SDK installed
- SQL Server or SQL Server LocalDB installed

## Step 1: Install Entity Framework Core Tools

If you haven't installed the EF Core tools globally, run:

```bash
dotnet tool install --global dotnet-ef
```

To update existing tools:

```bash
dotnet tool update --global dotnet-ef
```

## Step 2: Update Connection String

Edit `EmployeeManagementSystem2.Server/appsettings.json` and verify/update the connection string:

For LocalDB (default):
```json
"ConnectionStrings": {
  "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=EmployeeManagementDB;Trusted_Connection=True;MultipleActiveResultSets=true"
}
```

For SQL Server:
```json
"ConnectionStrings": {
  "DefaultConnection": "Server=YOUR_SERVER;Database=EmployeeManagementDB;User Id=YOUR_USER;Password=YOUR_PASSWORD;TrustServerCertificate=True"
}
```

## Step 3: Create Initial Migration

From the solution root directory, run:

```bash
dotnet ef migrations add InitialCreate --project EmployeeManagementSystem2.Server --startup-project EmployeeManagementSystem2.Server
```

This will create a `Migrations` folder with your initial migration.

## Step 4: Apply Migration to Database

Run the following command to create the database and apply the migration:

```bash
dotnet ef database update --project EmployeeManagementSystem2.Server --startup-project EmployeeManagementSystem2.Server
```

This will:
- Create the `EmployeeManagementDB` database
- Create all tables (Users, Employees, Departments, Roles)
- Seed initial data (4 Departments and 4 Roles)

## Step 5: Verify Database Creation

You can verify the database was created by:

### Using SQL Server Management Studio (SSMS)
1. Connect to your SQL Server instance
2. Look for `EmployeeManagementDB` database
3. Expand Tables to see: Departments, Employees, Roles, Users

### Using Command Line
```bash
dotnet ef database list --project EmployeeManagementSystem2.Server
```

## Seed Data

The following data is automatically seeded:

### Departments
- IT - Information Technology
- HR - Human Resources
- Finance - Finance Department
- Sales - Sales Department

### Roles
- Admin - System Administrator
- Manager - Department Manager
- Employee - Regular Employee
- Intern - Intern Employee

## Troubleshooting

### Issue: "dotnet-ef not found"
**Solution:** Install EF Core tools:
```bash
dotnet tool install --global dotnet-ef
```

### Issue: Connection to SQL Server failed
**Solution:** 
1. Verify SQL Server is running
2. Check connection string
3. Ensure user has appropriate permissions

### Issue: Build failed
**Solution:**
```bash
dotnet restore
dotnet build
```

### Issue: Migration already exists
**Solution:** Remove the migration and recreate:
```bash
dotnet ef migrations remove --project EmployeeManagementSystem2.Server
dotnet ef migrations add InitialCreate --project EmployeeManagementSystem2.Server
```

## Creating Additional Migrations

When you make changes to your models:

1. Create a new migration:
```bash
dotnet ef migrations add YourMigrationName --project EmployeeManagementSystem2.Server
```

2. Update the database:
```bash
dotnet ef database update --project EmployeeManagementSystem2.Server
```

## Removing Last Migration (before applying to database)

```bash
dotnet ef migrations remove --project EmployeeManagementSystem2.Server
```

## Resetting Database

To drop and recreate the database:

```bash
dotnet ef database drop --project EmployeeManagementSystem2.Server
dotnet ef database update --project EmployeeManagementSystem2.Server
```

## Production Deployment

For production:
1. Never use LocalDB
2. Use proper SQL Server instance
3. Update connection string in production configuration
4. Consider using SQL scripts instead of automatic migrations
5. Generate SQL scripts from migrations:
```bash
dotnet ef migrations script --project EmployeeManagementSystem2.Server --output migrations.sql
```
