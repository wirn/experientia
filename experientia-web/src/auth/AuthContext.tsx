import React, {
  createContext,
  useContext,
  useEffect,
  useMemo,
  useRef,
  useState,
} from "react";
import { api, setAuthToken } from "../api";

type AuthUser = { id: number; email: string; fullName: string };
type AuthState = { token: string | null; user: AuthUser | null };

type AuthContextValue = {
  user: AuthUser | null;
  token: string | null;
  isAuthenticated: boolean;
  isLoadingUser: boolean;
  login: (email: string, password: string) => Promise<void>;
  logout: () => void;
};

const KEY = "experientia_auth";

const AuthContext = createContext<AuthContextValue | undefined>(undefined);

/** Läs ut exp från ett JWT (returnerar epoch-seconds eller null) */
function getJwtExp(token: string | null): number | null {
  if (!token) return null;
  try {
    const payload = JSON.parse(atob(token.split(".")[1]));
    // exp enligt spec är i sekunder
    return typeof payload?.exp === "number" ? payload.exp : null;
  } catch {
    return null;
  }
}

/** Är token fortfarande giltigt just nu? */
function isTokenValidNow(token: string | null): boolean {
  const exp = getJwtExp(token);
  if (!exp) return !!token; // om token saknar exp antar vi giltigt
  const nowSec = Math.floor(Date.now() / 1000);
  return exp > nowSec;
}

export const AuthProvider: React.FC<React.PropsWithChildren> = ({
  children,
}) => {
  // 1) Läs ev. sparat läge från localStorage
  const [state, setState] = useState<AuthState>(() => {
    const raw = localStorage.getItem(KEY);
    return raw ? (JSON.parse(raw) as AuthState) : { token: null, user: null };
  });

  // Håller koll så vi inte kör parallella /me-anrop
  const loadingUserRef = useRef(false);
  const [isLoadingUser, setIsLoadingUser] = useState(false);

  // 2) Sätt global Authorization-header när token ändras
  useEffect(() => {
    setAuthToken(state.token);
  }, [state.token]);

  // 3) Axios-interceptor: logga ut på 401 (ogiltigt / utgånget token)
  useEffect(() => {
    const id = api.interceptors.response.use(
      (res) => res,
      (err) => {
        if (err?.response?.status === 401) {
          logout();
        }
        return Promise.reject(err);
      }
    );
    return () => api.interceptors.response.eject(id);
  }, []);

  // 4) Hydra användare vid uppstart om vi har ett giltigt token men ingen user
  useEffect(() => {
    if (!state.token) return; // inte inloggad

    // Är token redan ogiltigt? Nolla direkt.
    if (!isTokenValidNow(state.token)) {
      logout();
      return;
    }

    // Om vi har token men saknar user: hämta /auth/me
    if (!state.user && !loadingUserRef.current) {
      loadingUserRef.current = true;
      setIsLoadingUser(true);
      api
        .get<AuthUser>("/auth/me")
        .then((res) => {
          const next: AuthState = { token: state.token, user: res.data };
          localStorage.setItem(KEY, JSON.stringify(next));
          setState(next);
        })
        .catch(() => {
          // misslyckades att läsa user → logga ut
          logout();
        })
        .finally(() => {
          loadingUserRef.current = false;
          setIsLoadingUser(false);
        });
    }
  }, [state.token, state.user]);

  // 5) Login
  const login = async (email: string, password: string) => {
    const res = await api.post("/auth/login", { email, password });
    const { accessToken, user } = res.data as {
      accessToken: string;
      user: AuthUser;
    };

    // validera token snabbt (frivilligt)
    if (!isTokenValidNow(accessToken)) {
      throw new Error("Mottog ett ogiltigt eller utgånget token.");
    }

    const next: AuthState = { token: accessToken, user };
    localStorage.setItem(KEY, JSON.stringify(next));
    setState(next);
  };

  // 6) Logout
  const logout = () => {
    localStorage.removeItem(KEY);
    setState({ token: null, user: null });
    setAuthToken(null);
  };

  const value = useMemo<AuthContextValue>(
    () => ({
      user: state.user,
      token: state.token,
      isAuthenticated: !!state.token && !!state.user,
      isLoadingUser,
      login,
      logout,
    }),
    [state.user, state.token, isLoadingUser]
  );

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>;
};

export function useAuth() {
  const ctx = useContext(AuthContext);
  if (!ctx) throw new Error("useAuth must be used within AuthProvider");
  return ctx;
}
