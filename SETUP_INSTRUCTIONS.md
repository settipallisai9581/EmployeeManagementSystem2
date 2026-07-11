# Employee Management System

A full-stack Employee Management System built with .NET 8 Web API and React with TypeScript.

## Features

- **Authentication & Authorization**
  - User registration and login
  - JWT-based authentication
  - Secure password hashing with BCrypt

- **Employee Management (CRUD Operations)**
  - Create new employees
  - View employee list with search functionality
  - View detailed employee information
  - Edit employee information
  - Delete employees
  - Upload employee photos

- **Department & Role Management**
  - Pre-seeded departments (IT, HR, Finance, Sales)
  - Pre-seeded roles (Admin, Manager, Employee, Intern)

## Tech Stack

### Backend
- .NET 8
- Entity Framework Core
- SQL Server (LocalDB)
- JWT Authentication
- BCrypt for password hashing
- Swagger/OpenAPI

### Frontend
- React 19
- TypeScript
- React Router for navigation
- Axios for API calls
- Vite for build tooling

## Prerequisites

- .NET 8 SDK
- Node.js (v18 or higher)
- SQL Server LocalDB (comes with Visual Studio)

## Setup Instructions

### 1. Clone the Repository

```bash
cd "C:\Users\ssaikumar\source\repos\EmployeeManagementSystem2"
```

### 2. Database Setup

The application uses SQL Server LocalDB. The connection string is configured in `appsettings.json`:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=EmployeeManagementDB;Trusted_Connection=True;MultipleActiveResultSets=true"
}
```

Create and seed the database:

```bash
cd EmployeeManagementSystem2.Server
dotnet ef database update
```

If you don't have the EF Core CLI tools installed:

```bash
dotnet tool install --global dotnet-ef
```

### 3. Backend Setup

```bash
cd EmployeeManagementSystem2.Server
dotnet restore
dotnet build
```

### 4. Frontend Setup

```bash
cd employeemanagementsystem2.client
npm install
```

### 5. Run the Application

#### Option 1: Run from Visual Studio
- Open `EmployeeManagementSystem2.sln` in Visual Studio
- Press F5 to run the application
- The backend API will start on `https://localhost:7XXX`
- The React frontend will start on `https://localhost:56883`

#### Option 2: Run from Command Line

**Terminal 1 - Backend:**
```bash
cd EmployeeManagementSystem2.Server
dotnet run
```

**Terminal 2 - Frontend:**
```bash
cd employeemanagementsystem2.client
npm run dev
```

## Default Data

The application seeds the following data on first run:

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

## API Endpoints

### Authentication
- `POST /api/auth/login` - User login
- `POST /api/auth/register` - User registration

### Employees
- `GET /api/employees` - Get all employees
- `GET /api/employees/{id}` - Get employee by ID
- `GET /api/employees/search?searchTerm={term}` - Search employees
- `POST /api/employees` - Create new employee
- `PUT /api/employees/{id}` - Update employee
- `DELETE /api/employees/{id}` - Delete employee
- `POST /api/employees/{id}/photo` - Upload employee photo
- `GET /api/employees/report` - Get employee statistics

### Departments
- `GET /api/departments` - Get all departments
- `GET /api/departments/{id}` - Get department by ID
- `POST /api/departments` - Create department
- `PUT /api/departments/{id}` - Update department
- `DELETE /api/departments/{id}` - Delete department

### Roles
- `GET /api/roles` - Get all roles
- `GET /api/roles/{id}` - Get role by ID
- `POST /api/roles` - Create role
- `PUT /api/roles/{id}` - Update role
- `DELETE /api/roles/{id}` - Delete role

## Testing the Application

### 1. Register a New User
1. Navigate to `https://localhost:56883/register`
2. Fill in the registration form:
   - Username (minimum 3 characters)
   - Email
   - First Name & Last Name
   - Select Department
   - Select Role
   - Password (minimum 6 characters)
3. Click "Register"

### 2. Login
1. Navigate to `https://localhost:56883/login`
2. Enter your email and password
3. Click "Login"

### 3. Manage Employees
- **View Employees:** Click "Employees" in the navigation
- **Add Employee:** Click "Add New Employee" button
- **Search:** Use the search bar to filter employees
- **View Details:** Click "View" button on any employee
- **Edit:** Click "Edit" button
- **Delete:** Click "Delete" button (with confirmation)

## Project Structure

```
EmployeeManagementSystem2/
??? EmployeeManagementSystem2.Server/          # Backend .NET API
?   ??? Controllers/                            # API Controllers
?   ??? Data/                                   # DbContext
?   ??? DTOs/                                   # Data Transfer Objects
?   ??? Models/                                 # Entity Models
?   ??? Services/                               # Business Logic Services
?   ??? Program.cs                              # App configuration
?
??? employeemanagementsystem2.client/          # Frontend React App
    ??? src/
    ?   ??? components/                         # Reusable components
    ?   ??? context/                            # React Context (Auth)
    ?   ??? pages/                              # Page components
    ?   ??? services/                           # API service layer
    ?   ??? styles/                             # CSS files
    ?   ??? types/                              # TypeScript interfaces
    ?   ??? App.tsx                             # Main app component
    ?   ??? main.tsx                            # Entry point
    ??? package.json
```

## Security Features

- Passwords are hashed using BCrypt before storage
- JWT tokens for stateless authentication
- Token expiration (8 hours)
- Protected routes on the frontend
- CORS configuration for API security
- Input validation on both client and server

## Troubleshooting

### Database Connection Issues
- Ensure SQL Server LocalDB is installed
- Check if the database was created: `dotnet ef database update`
- Verify connection string in `appsettings.json`

### Port Already in Use
- Change the ports in `launchSettings.json` (backend)
- Change the port in `vite.config.ts` (frontend)

### NPM/Node Issues
- Clear npm cache: `npm cache clean --force`
- Delete `node_modules` and `package-lock.json`, then run `npm install`

### Build Errors
- Clean and rebuild: `dotnet clean && dotnet build`
- Restore packages: `dotnet restore`

## Future Enhancements

- Role-based authorization (Admin, Manager, Employee)
- Employee performance reviews
- Leave management system
- Attendance tracking
- Reporting and analytics dashboard
- Email notifications
- Document management
- Multi-tenant support

## License

This project is for educational purposes.
