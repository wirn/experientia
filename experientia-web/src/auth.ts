export type AuthUser = { id: number; email: string; fullName: string };
export type AuthState = { token: string | null; user: AuthUser | null };

const KEY = "experientia_auth";

export function loadAuth(): AuthState {
  const raw = localStorage.getItem(KEY);
  return raw ? JSON.parse(raw) : { token: null, user: null };
}

export function saveAuth(s: AuthState) {
  localStorage.setItem(KEY, JSON.stringify(s));
}
export function clearAuth() {
  localStorage.removeItem(KEY);
}
