export interface Employee {
  id: number;
  firstName: string;
  lastName: string;
  email: string;
  phone?: string;
  dateOfBirth?: string;
  hireDate: string;
  address?: string;
  city?: string;
  state?: string;
  zipCode?: string;
  photoPath?: string;
  salary?: number;
  isActive: boolean;
  departmentId: number;
  departmentName: string;
  roleId: number;
  roleName: string;
}

export interface CreateEmployeeRequest {
  firstName: string;
  lastName: string;
  email: string;
  phone?: string;
  dateOfBirth?: string;
  hireDate: string;
  address?: string;
  city?: string;
  state?: string;
  zipCode?: string;
  salary?: number;
  departmentId: number;
  roleId: number;
}

export interface UpdateEmployeeRequest extends CreateEmployeeRequest {
  isActive: boolean;
}

export interface Department {
  id: number;
  name: string;
  description?: string;
}

export interface Role {
  id: number;
  name: string;
  description?: string;
}

export interface LoginRequest {
  email: string;
  password: string;
}

export interface RegisterRequest {
  username: string;
  email: string;
  password: string;
  firstName: string;
  lastName: string;
  departmentId: number;
  roleId: number;
}

export interface AuthResponse {
  token: string;
  username: string;
  email: string;
  userId: number;
  employeeId?: number;
}

export interface User {
  token: string;
  username: string;
  email: string;
  userId: number;
  employeeId?: number;
}
