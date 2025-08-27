import { Outlet } from "react-router-dom";

export default function App() {
  return (
    <div
      style={{ maxWidth: 480, margin: "40px auto", fontFamily: "system-ui" }}
    >
      {/* <nav style={{ display: "flex", gap: 12, marginBottom: 24 }}>
        <Link to="/login">Login</Link>
        <Link to="/register">Register</Link>
      </nav> */}
      <Outlet />
    </div>
  );
}
