import { useState, useEffect, useCallback, useRef } from 'react';
import type { ReactNode } from 'react';
import type { User, LoginRequest, RegisterRequest } from '../types';
import { authApi } from '../services/api';
import { clientLogger } from '../services/logger';
import { AuthContext } from './auth-context';
import {
  LAST_ACTIVITY_AT_KEY,
  SESSION_TIMEOUT_EVENT,
  SESSION_TIMEOUT_FLAG_KEY,
  SESSION_TIMEOUT_MS,
  restoreUserFromStorage,
} from './auth-storage';

export const AuthProvider = ({ children }: { children: ReactNode }) => {
  const [user, setUser] = useState<User | null>(restoreUserFromStorage);
  const [sessionTimedOut, setSessionTimedOut] = useState(
    () => localStorage.getItem(SESSION_TIMEOUT_FLAG_KEY) === 'true'
  );
  const timeoutRef = useRef<number | null>(null);
  const lastActivityUpdateRef = useRef(0);

  const clearSessionTimer = useCallback(() => {
    if (timeoutRef.current !== null) {
      window.clearTimeout(timeoutRef.current);
      timeoutRef.current = null;
    }
  }, []);

  const clearAuthStorage = useCallback(() => {
    localStorage.removeItem('token');
    localStorage.removeItem('user');
  }, []);

  const markSessionActivity = useCallback(() => {
    localStorage.setItem(LAST_ACTIVITY_AT_KEY, Date.now().toString());
  }, []);

  const clearSessionTimedOutState = useCallback(() => {
    localStorage.removeItem(SESSION_TIMEOUT_FLAG_KEY);
    setSessionTimedOut(false);
  }, []);

  const triggerSessionTimeout = useCallback(() => {
    if (!localStorage.getItem('token')) {
      return;
    }

    clientLogger.warn('Session timed out due to inactivity or token expiry');
    clearSessionTimer();
    clearAuthStorage();
    localStorage.removeItem(LAST_ACTIVITY_AT_KEY);
    localStorage.setItem(SESSION_TIMEOUT_FLAG_KEY, 'true');
    setUser(null);
    setSessionTimedOut(true);
  }, [clearAuthStorage, clearSessionTimer]);

  const scheduleSessionTimeout = useCallback(() => {
    if (!localStorage.getItem('token')) {
      clearSessionTimer();
      return;
    }

    const lastActivityAt = Number(localStorage.getItem(LAST_ACTIVITY_AT_KEY) ?? Date.now());
    const elapsedMs = Date.now() - lastActivityAt;
    const remainingMs = SESSION_TIMEOUT_MS - elapsedMs;

    clearSessionTimer();

    if (remainingMs <= 0) {
      triggerSessionTimeout();
      return;
    }

    timeoutRef.current = window.setTimeout(() => {
      triggerSessionTimeout();
    }, remainingMs);
  }, [clearSessionTimer, triggerSessionTimeout]);

  useEffect(() => {
    if (!user) {
      clearSessionTimer();
      return;
    }

    const initialScheduleId = window.setTimeout(() => {
      scheduleSessionTimeout();
    }, 0);

    const onActivity = () => {
      const now = Date.now();
      if (now - lastActivityUpdateRef.current < 1000) {
        return;
      }

      lastActivityUpdateRef.current = now;
      markSessionActivity();
      scheduleSessionTimeout();
    };

    const activityEvents: Array<keyof WindowEventMap> = [
      'click',
      'keydown',
      'mousemove',
      'scroll',
      'touchstart',
    ];

    activityEvents.forEach((eventName) => {
      window.addEventListener(eventName, onActivity, { passive: true });
    });

    return () => {
      window.clearTimeout(initialScheduleId);
      activityEvents.forEach((eventName) => {
        window.removeEventListener(eventName, onActivity);
      });
      clearSessionTimer();
    };
  }, [user, clearSessionTimer, markSessionActivity, scheduleSessionTimeout]);

  useEffect(() => {
    const onSessionTimeout = () => {
      triggerSessionTimeout();
    };

    window.addEventListener(SESSION_TIMEOUT_EVENT, onSessionTimeout);

    return () => {
      window.removeEventListener(SESSION_TIMEOUT_EVENT, onSessionTimeout);
    };
  }, [triggerSessionTimeout]);

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
    markSessionActivity();
    clearSessionTimedOutState();
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
    markSessionActivity();
    clearSessionTimedOutState();
    setUser(userData);

    clientLogger.info('Register action completed', { username: response.username, userId: response.userId });
  };

  const logout = () => {
    clientLogger.info('Logout action started');
    clearSessionTimer();
    clearAuthStorage();
    localStorage.removeItem(LAST_ACTIVITY_AT_KEY);
    clearSessionTimedOutState();
    setUser(null);
    clientLogger.info('Logout action completed');
  };

  return (
    <AuthContext.Provider
      value={{ user, login, register, logout, isAuthenticated: !!user, sessionTimedOut }}
    >
      {children}
    </AuthContext.Provider>
  );
};
