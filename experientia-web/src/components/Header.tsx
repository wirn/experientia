import { useAuth } from "../auth/AuthContext";
import { FaUser } from "react-icons/fa";
import { IoLogIn, IoLogOut } from "react-icons/io5";
import styles from "./Header.module.scss";

export default function Header() {
  const { isAuthenticated, user, logout, isLoadingUser } = useAuth();

  return (
    <header className={styles.navbarTop}>
      <nav className="container py-2">
        <h1 className="mb-0">Experientia</h1>
        {isLoadingUser ? (
          <span>laddar…</span>
        ) : isAuthenticated ? (
          <div className="d-flex justify-content-between align-items-center me-3">
            <span className="me-2">
              <FaUser size={20} /> {user?.fullName || user?.email}
            </span>
            <button className="btn btn-link" onClick={logout}>
              <IoLogOut size={16} className="me-1" /> Logga ut
            </button>
          </div>
        ) : (
          <a className="btn btn-link" href="/login">
            <IoLogIn className="me-1" size={16} />
            Logga in
          </a>
        )}
      </nav>
    </header>
  );
}
