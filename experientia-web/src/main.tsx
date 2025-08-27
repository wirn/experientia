// main.tsx (eller där du har din router)
import React from "react";
import ReactDOM from "react-dom/client";
import {
  createBrowserRouter,
  Navigate,
  RouterProvider,
} from "react-router-dom";
import { AuthProvider } from "./auth/AuthContext";

import { RequireAuth, RequireGuest } from "./auth/RouteGuard";

import App from "./App";
import Header from "./components/Header";

import Login from "./pages/Login";
import Register from "./pages/Register";
import Dashboard from "./pages/Dashboard";

import "./style/app.scss";

const router = createBrowserRouter([
  {
    element: <RequireGuest />,
    children: [
      { path: "/", element: <Navigate to="/login" replace /> },
      { path: "/login", element: <Login /> },
      { path: "/register", element: <Register /> },
    ],
  },

  {
    element: <RequireAuth />,
    children: [
      {
        path: "/",
        element: <App />,
        children: [{ path: "dashboard", element: <Dashboard /> }],
      },
    ],
  },

  { path: "*", element: <Navigate to="/" replace /> },
]);

ReactDOM.createRoot(document.getElementById("root")!).render(
  <React.StrictMode>
    <AuthProvider>
      <Header />
      <div className="container">
        <RouterProvider router={router} />
      </div>
    </AuthProvider>
  </React.StrictMode>
);
