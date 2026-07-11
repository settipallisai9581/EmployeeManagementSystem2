import axios from 'axios';
import type {
  Employee,
  CreateEmployeeRequest,
  UpdateEmployeeRequest,
  Department,
  Role,
  LoginRequest,
  RegisterRequest,
  AuthResponse,
} from '../types';

const api = axios.create({
  baseURL: '/api',
  headers: {
    'Content-Type': 'application/json',
  },
});

const ensureArrayResponse = <T>(data: unknown, resourceName: string): T[] => {
  if (Array.isArray(data)) {
    return data as T[];
  }

  // Support serializers that wrap collections as { $values: [...] }
  if (
    data &&
    typeof data === 'object' &&
    '$values' in data &&
    Array.isArray((data as { $values: unknown[] }).$values)
  ) {
    return (data as { $values: T[] }).$values;
  }

  throw new Error(`Unexpected ${resourceName} response format`);
};

api.interceptors.request.use((config) => {
  const token = localStorage.getItem('token');
  if (token) {
    config.headers.Authorization = `Bearer ${token}`;
  }
  return config;
});

export const authApi = {
  login: async (data: LoginRequest): Promise<AuthResponse> => {
    const response = await api.post<AuthResponse>('/auth/login', data);
    return response.data;
  },

  register: async (data: RegisterRequest): Promise<AuthResponse> => {
    const response = await api.post<AuthResponse>('/auth/register', data);
    return response.data;
  },
};

export const employeeApi = {
  getAll: async (): Promise<Employee[]> => {
    const response = await api.get<Employee[]>('/employees');
    return response.data;
  },

  getById: async (id: number): Promise<Employee> => {
    const response = await api.get<Employee>(`/employees/${id}`);
    return response.data;
  },

  search: async (searchTerm: string): Promise<Employee[]> => {
    const response = await api.get<Employee[]>('/employees/search', {
      params: { searchTerm },
    });
    return response.data;
  },

  create: async (data: CreateEmployeeRequest): Promise<Employee> => {
    const response = await api.post<Employee>('/employees', data);
    return response.data;
  },

  update: async (id: number, data: UpdateEmployeeRequest): Promise<Employee> => {
    const response = await api.put<Employee>(`/employees/${id}`, data);
    return response.data;
  },

  delete: async (id: number): Promise<void> => {
    await api.delete(`/employees/${id}`);
  },

  uploadPhoto: async (id: number, file: File): Promise<{ photoPath: string }> => {
    const formData = new FormData();
    formData.append('file', file);
    const response = await api.post<{ photoPath: string }>(
      `/employees/${id}/photo`,
      formData,
      {
        headers: {
          'Content-Type': 'multipart/form-data',
        },
      }
    );
    return response.data;
  },
};

export const departmentApi = {
  getAll: async (): Promise<Department[]> => {
    const response = await api.get<unknown>('/departments');
    return ensureArrayResponse<Department>(response.data, 'departments');
  },
};

export const roleApi = {
  getAll: async (): Promise<Role[]> => {
    const response = await api.get<unknown>('/roles');
    return ensureArrayResponse<Role>(response.data, 'roles');
  },
};
