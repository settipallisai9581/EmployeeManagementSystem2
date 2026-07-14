import { createContext, useContext, useState, useEffect } from 'react';
import type { ReactNode } from 'react';
import type { User, LoginRequest, RegisterRequest } from '../types';
import { authApi } from '../services/api';
import { clientLogger } from '../services/logger';

interface AuthContextType {
  user: User | null;
  login: (data: LoginRequest) => Promise<void>;
  register: (data: RegisterRequest) => Promise<void>;
  logout: () => void;
  isAuthenticated: boolean;
}

const AuthContext = createContext<AuthContextType | undefined>(undefined);

export const AuthProvider = ({ children }: { children: ReactNode }) => {
  const [user, setUser] = useState<User | null>(null);

  useEffect(() => {
    clientLogger.info('Auth context initialization started');

    const token = localStorage.getItem('token');
    const userStr = localStorage.getItem('user');
    if (token && userStr) {
      try {
        setUser(JSON.parse(userStr));
        clientLogger.info('Auth session restored from local storage');
      } catch {
        localStorage.removeItem('token');
        localStorage.removeItem('user');
        clientLogger.warn('Auth session restore failed; storage values were cleared');
      }
    }

    clientLogger.info('Auth context initialization completed');
  }, []);

  const login = async (data: LoginRequest) => {
    clientLogger.info('Login action started', { email: data.email });
    const response = await authApi.login(data);
    const userData: User = {
      token: response.token,
      username: response.username,
      email: response.email,
      userId: response.userId,
      employeeId: response.employeeId,
    };
    localStorage.setItem('token', response.token);
    localStorage.setItem('user', JSON.stringify(userData));
    setUser(userData);
    clientLogger.info('Login action completed', { username: response.username, userId: response.userId });
  };

  const register = async (data: RegisterRequest) => {
    clientLogger.info('Register action started', { username: data.username, email: data.email });
    const response = await authApi.register(data);
    const userData: User = {
      token: response.token,
      username: response.username,
      email: response.email,
      userId: response.userId,
      employeeId: response.employeeId,
    };
    localStorage.setItem('token', response.token);
    localStorage.setItem('user', JSON.stringify(userData));
    setUser(userData);
    clientLogger.info('Register action completed', { username: response.username, userId: response.userId });
  };

  const logout = () => {
    clientLogger.info('Logout action started');
    localStorage.removeItem('token');
    localStorage.removeItem('user');
    setUser(null);
    clientLogger.info('Logout action completed');
  };

  return (
    <AuthContext.Provider
      value={{ user, login, register, logout, isAuthenticated: !!user }}
    >
      {children}
    </AuthContext.Provider>
  );
};

export const useAuth = () => {
  const context = useContext(AuthContext);
  if (context === undefined) {
    throw new Error('useAuth must be used within an AuthProvider');
  }
  return context;
};
