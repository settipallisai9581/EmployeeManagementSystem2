import { createContext } from 'react';
import type { LoginRequest, RegisterRequest, User } from '../types';

export interface AuthContextType {
  user: User | null;
  login: (data: LoginRequest) => Promise<void>;
  register: (data: RegisterRequest) => Promise<void>;
  logout: () => void;
  isAuthenticated: boolean;
  sessionTimedOut: boolean;
}

export const AuthContext = createContext<AuthContextType | undefined>(undefined);
