import type { User } from '../types';
import { clientLogger } from '../services/logger';

export const SESSION_TIMEOUT_MS = 2 * 60 * 1000;
export const SESSION_TIMEOUT_FLAG_KEY = 'sessionTimedOut';
export const LAST_ACTIVITY_AT_KEY = 'lastActivityAt';
export const SESSION_TIMEOUT_EVENT = 'auth:session-timeout';

export const restoreUserFromStorage = (): User | null => {
  clientLogger.info('Auth context initialization started');

  const token = localStorage.getItem('token');
  const userStr = localStorage.getItem('user');
  if (!token || !userStr) {
    clientLogger.info('Auth context initialization completed');
    return null;
  }

  try {
    const lastActivityAt = Number(localStorage.getItem(LAST_ACTIVITY_AT_KEY) ?? Date.now());
    if (Date.now() - lastActivityAt >= SESSION_TIMEOUT_MS) {
      localStorage.removeItem('token');
      localStorage.removeItem('user');
      localStorage.removeItem(LAST_ACTIVITY_AT_KEY);
      localStorage.setItem(SESSION_TIMEOUT_FLAG_KEY, 'true');
      clientLogger.warn('Restored session was already timed out');
      clientLogger.info('Auth context initialization completed');
      return null;
    }

    localStorage.setItem(LAST_ACTIVITY_AT_KEY, Date.now().toString());
    localStorage.removeItem(SESSION_TIMEOUT_FLAG_KEY);
    const user = JSON.parse(userStr) as User;
    clientLogger.info('Auth session restored from local storage');
    clientLogger.info('Auth context initialization completed');
    return user;
  } catch {
    localStorage.removeItem('token');
    localStorage.removeItem('user');
    localStorage.removeItem(LAST_ACTIVITY_AT_KEY);
    localStorage.removeItem(SESSION_TIMEOUT_FLAG_KEY);
    clientLogger.warn('Auth session restore failed; storage values were cleared');
    clientLogger.info('Auth context initialization completed');
    return null;
  }
};
