import axios from "axios";

//const port = "7279";

export const api = axios.create({
  baseURL: `https://localhost:7279/api`,
  withCredentials: false,
});

export function setAuthToken(token: string | null) {
  if (token) api.defaults.headers.common["Authorization"] = `Bearer ${token}`;
  else delete api.defaults.headers.common["Authorization"];
}
