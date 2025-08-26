import React, {
  createContext,
  useContext,
  useEffect,
  useMemo,
  useState,
} from "react";
import { api, setAuthToken } from "../api";

type AuthUser = { id: number; email: string; fullName: string };
type AuthState = { token: string | null; user: AuthUser | null };

type AuthContextValue = {
  user: AuthUser | null;
  token: string | null;
  isAuthenticated: boolean;
  login: (email: string, password: string) => Promise<void>;
  logout: () => void;
};

const KEY = "experientia_auth";

const AuthContext = createContext<AuthContextValue | undefined>(undefined);

export const AuthProvider: React.FC<React.PropsWithChildren> = ({
  children,
}) => {
  const [state, setState] = useState<AuthState>(() => {
    const raw = localStorage.getItem(KEY);
    return raw ? (JSON.parse(raw) as AuthState) : { token: null, user: null };
  });

  // sätt axios header när state ändras
  useEffect(() => {
    setAuthToken(state.token);
  }, [state.token]);

  const login = async (email: string, password: string) => {
    const res = await api.post("/auth/login", { email, password });
    const { accessToken, user } = res.data as {
      accessToken: string;
      user: AuthUser;
    };
    const next: AuthState = { token: accessToken, user };
    localStorage.setItem(KEY, JSON.stringify(next));
    setState(next);
  };

  const logout = () => {
    localStorage.removeItem(KEY);
    setState({ token: null, user: null });
    setAuthToken(null);
  };

  const value = useMemo<AuthContextValue>(
    () => ({
      user: state.user,
      token: state.token,
      isAuthenticated: !!state.token,
      login,
      logout,
    }),
    [state.user, state.token]
  );

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>;
};

export function useAuth() {
  const ctx = useContext(AuthContext);
  if (!ctx) throw new Error("useAuth must be used within AuthProvider");
  return ctx;
}
