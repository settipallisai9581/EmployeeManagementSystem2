# Employee Management System

A complete employee management system built with .NET 8 and Entity Framework Core.

## Quick Start (Run Client + Server)

After cloning the repo, run both backend and frontend:

### Option A: VS Code Tasks (recommended)

1. Open the repository in VS Code.
2. Run task `Run Client + Server`.
3. Server starts on `http://localhost:5187`.
4. Client opens in browser on Vite port (default `https://localhost:56883`, auto-switches if occupied).

### Option B: Two terminals

Terminal 1 (server):

```bash
dotnet run --project EmployeeManagementSystem2.Server/EmployeeManagementSystem2.Server.csproj
```

Terminal 2 (client):

```bash
cd employeemanagementsystem2.client
npm install
npm run dev
```

### API and UI URLs

- API Swagger: `http://localhost:5187/swagger`
- Client UI: `https://localhost:56883` (or next available Vite port)

## Features

- **User Authentication**
  - User Registration
  - User Login with JWT Token
  - Secure Password Hashing with BCrypt

- **Employee Management**
  - Create, Read, Update, Delete (CRUD) operations
  - Employee photo upload
  - Search employees by name, email, phone, department, or role
  - View employee details

- **Department Management**
  - Create, Read, Update, Delete departments
  - View employees by department
  - Prevent deletion of departments with employees

- **Role Management**
  - Create, Read, Update, Delete roles
  - View employees by role
  - Prevent deletion of roles with employees

- **Reports**
  - Total, active, and inactive employee counts
  - Employee distribution by department
  - Employee distribution by role
  - Average and total salary calculations

## Technology Stack

- .NET 8
- ASP.NET Core Web API
- Entity Framework Core 8
- SQL Server
- JWT Authentication
- BCrypt for password hashing
- Swagger/OpenAPI

## Prerequisites

- .NET 8 SDK
- SQL Server or SQL Server LocalDB
- Visual Studio 2022 or VS Code

## Setup Instructions

### 1. Restore NuGet Packages

```bash
dotnet restore
```

### 2. Update Database Connection String

Edit `appsettings.json` and update the connection string if needed:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=EmployeeManagementDB;Trusted_Connection=True;MultipleActiveResultSets=true"
}
```

### 3. Create Database Migration

```bash
dotnet ef migrations add InitialCreate --project EmployeeManagementSystem2.Server
```

### 4. Update Database

```bash
dotnet ef database update --project EmployeeManagementSystem2.Server
```

This will create the database with seed data:
- 4 Departments: IT, HR, Finance, Sales
- 4 Roles: Admin, Manager, Employee, Intern

### 5. Run the Application

```bash
dotnet run --project EmployeeManagementSystem2.Server
```

The API will be available at:
- HTTPS: `https://localhost:5001`
- HTTP: `http://localhost:5000`
- Swagger UI: `https://localhost:5001/swagger`

## API Endpoints

### Authentication

- `POST /api/auth/register` - Register a new user
- `POST /api/auth/login` - Login and get JWT token

### Employees

- `GET /api/employees` - Get all employees
- `GET /api/employees/{id}` - Get employee by ID
- `GET /api/employees/search?searchTerm={term}` - Search employees
- `POST /api/employees` - Create new employee
- `PUT /api/employees/{id}` - Update employee
- `DELETE /api/employees/{id}` - Delete employee
- `POST /api/employees/{id}/photo` - Upload employee photo
- `GET /api/employees/report` - Get employee report

### Departments

- `GET /api/departments` - Get all departments
- `GET /api/departments/{id}` - Get department by ID
- `POST /api/departments` - Create new department
- `PUT /api/departments/{id}` - Update department
- `DELETE /api/departments/{id}` - Delete department

### Roles

- `GET /api/roles` - Get all roles
- `GET /api/roles/{id}` - Get role by ID
- `POST /api/roles` - Create new role
- `PUT /api/roles/{id}` - Update role
- `DELETE /api/roles/{id}` - Delete role

## Authentication

All endpoints except `/api/auth/login` and `/api/auth/register` require authentication.

1. Register or login to get a JWT token
2. Add the token to the Authorization header:
   ```
   Authorization: Bearer {your-token}
   ```

## Example Usage

### Register a User

```bash
POST /api/auth/register
Content-Type: application/json

{
  "username": "john.doe",
  "email": "john.doe@example.com",
  "password": "SecurePassword123",
  "firstName": "John",
  "lastName": "Doe",
  "departmentId": 1,
  "roleId": 3
}
```

### Login

```bash
POST /api/auth/login
Content-Type: application/json

{
  "email": "john.doe@example.com",
  "password": "SecurePassword123"
}
```

Response:
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "username": "john.doe",
  "email": "john.doe@example.com",
  "userId": 1,
  "employeeId": 1
}
```

### Create an Employee

```bash
POST /api/employees
Authorization: Bearer {your-token}
Content-Type: application/json

{
  "firstName": "Jane",
  "lastName": "Smith",
  "email": "jane.smith@example.com",
  "phone": "555-0123",
  "dateOfBirth": "1990-05-15",
  "hireDate": "2024-01-01",
  "address": "123 Main St",
  "city": "New York",
  "state": "NY",
  "zipCode": "10001",
  "salary": 75000,
  "departmentId": 1,
  "roleId": 3
}
```

### Search Employees

```bash
GET /api/employees/search?searchTerm=john
Authorization: Bearer {your-token}
```

### Upload Employee Photo

```bash
POST /api/employees/1/photo
Authorization: Bearer {your-token}
Content-Type: multipart/form-data

file: [image file]
```

### Get Employee Report

```bash
GET /api/employees/report
Authorization: Bearer {your-token}
```

## Photo Upload

- Supported formats: JPG, JPEG, PNG, GIF
- Maximum file size: 5MB
- Photos are stored in `wwwroot/uploads/photos/`

## Database Schema

### Users
- Id, Username, Email, PasswordHash, CreatedDate, LastLoginDate, IsActive

### Employees
- Id, FirstName, LastName, Email, Phone, DateOfBirth, HireDate
- Address, City, State, ZipCode, PhotoPath, Salary, IsActive
- DepartmentId (FK), RoleId (FK), UserId (FK)

### Departments
- Id, Name, Description, CreatedDate, UpdatedDate, IsActive

### Roles
- Id, Name, Description, CreatedDate, UpdatedDate, IsActive

## Security Features

- Password hashing with BCrypt
- JWT token-based authentication
- Protected API endpoints
- Input validation
- Email uniqueness validation

## Error Handling

The API returns appropriate HTTP status codes:
- 200 OK - Success
- 201 Created - Resource created
- 204 No Content - Successful deletion
- 400 Bad Request - Invalid input
- 401 Unauthorized - Authentication required
- 404 Not Found - Resource not found

## Development

To make changes to the database schema:

1. Update the models in `Models/` folder
2. Create a new migration:
   ```bash
   dotnet ef migrations add YourMigrationName --project EmployeeManagementSystem2.Server
   ```
3. Update the database:
   ```bash
   dotnet ef database update --project EmployeeManagementSystem2.Server
   ```

## License

This project is for educational and demonstration purposes.
