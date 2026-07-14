type ClientLogLevel = 'info' | 'warn' | 'error' | 'debug';

interface ClientLogPayload {
  level: ClientLogLevel;
  message: string;
  metadata?: unknown;
}

const sessionStorageKey = 'clientLogSessionId';

const getSessionId = (): string => {
  const existing = sessionStorage.getItem(sessionStorageKey);
  if (existing) {
    return existing;
  }

  const generated = `${Date.now()}-${Math.random().toString(36).slice(2, 10)}`;
  sessionStorage.setItem(sessionStorageKey, generated);
  return generated;
};

const safeSerialize = (value: unknown): unknown => {
  if (!value || typeof value !== 'object') {
    return value;
  }

  const blockedKeys = new Set(['password', 'token', 'authorization']);
  const output: Record<string, unknown> = {};
  for (const [key, raw] of Object.entries(value as Record<string, unknown>)) {
    if (blockedKeys.has(key.toLowerCase())) {
      output[key] = '[REDACTED]';
      continue;
    }

    output[key] = raw;
  }

  return output;
};

const sendToServer = async (payload: ClientLogPayload): Promise<void> => {
  try {
    await fetch('/api/logs/client', {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
      },
      keepalive: true,
      body: JSON.stringify({
        level: payload.level,
        message: payload.message,
        route: window.location.pathname,
        sessionId: getSessionId(),
        metadata: safeSerialize(payload.metadata),
        loggedAtUtc: new Date().toISOString(),
      }),
    });
  } catch {
    // Best-effort forwarding to server logging endpoint.
  }
};

const log = (level: ClientLogLevel, message: string, metadata?: unknown): void => {
  const serializedMetadata = safeSerialize(metadata);

  if (level === 'error') {
    console.error(`[client:${level}] ${message}`, serializedMetadata);
  } else if (level === 'warn') {
    console.warn(`[client:${level}] ${message}`, serializedMetadata);
  } else if (level === 'debug') {
    console.debug(`[client:${level}] ${message}`, serializedMetadata);
  } else {
    console.info(`[client:${level}] ${message}`, serializedMetadata);
  }

  void sendToServer({ level, message, metadata: serializedMetadata });
};

export const clientLogger = {
  info: (message: string, metadata?: unknown) => log('info', message, metadata),
  warn: (message: string, metadata?: unknown) => log('warn', message, metadata),
  error: (message: string, metadata?: unknown) => log('error', message, metadata),
  debug: (message: string, metadata?: unknown) => log('debug', message, metadata),
};