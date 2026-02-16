/**
 * API Client - Base fetch wrapper with error handling
 */

export class ApiError extends Error {
  public status: number;
  public body?: unknown;

  constructor(
    message: string,
    status: number,
    body?: unknown
  ) {
    super(message);
    this.name = 'ApiError';
    this.status = status;
    this.body = body;
  }
}

const getBaseUrl = (): string => {
  const raw = import.meta.env.VITE_API_URL || 'http://localhost:5075';
  // remove trailing slashes
  return raw.replace(/\/+$|\/+$/g, '').replace(/\/$/, '');
};

export async function fetchJson<T>(path: string, init?: RequestInit): Promise<T> {
  const baseUrl = getBaseUrl();

  const normalizePath = (p: string) => {
    if (!p) return '/';
    return p.startsWith('/') ? p : '/' + p;
  };

  const normalizedPath = normalizePath(path);
  const url = `${baseUrl}${normalizedPath}`;

  // debug URL
  // eslint-disable-next-line no-console
  console.log('[API]', url);

  const headers = new Headers(init?.headers || {});

  // Set content-type for JSON requests if body is provided
  if (init?.body && !headers.has('Content-Type')) {
    headers.set('Content-Type', 'application/json');
  }

  const response = await fetch(url, {
    ...init,
    headers,
  });

  if (!response.ok) {
    let body: unknown;
    try {
      body = await response.json();
    } catch {
      body = await response.text();
    }

    const message = `${response.status} ${response.statusText}`;
    throw new ApiError(message, response.status, body);
  }

  // Handle empty/no-content responses
  if (response.status === 204) {
    return (undefined as unknown) as T;
  }

  const contentType = response.headers.get('content-type') || '';
  if (!contentType.includes('application/json')) {
    const text = await response.text();
    throw new ApiError(`Expected JSON but got: ${contentType || 'unknown'}`, response.status, text);
  }

  return response.json() as Promise<T>;
}
