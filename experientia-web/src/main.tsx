import React from "react";
import ReactDOM from "react-dom/client";
import {
  createBrowserRouter,
  RouterProvider,
  //Navigate,
} from "react-router-dom";
import App from "./App";
import Login from "./pages/Login";
import Register from "./pages/Register";
import { AuthProvider } from "./auth/AuthContext";

const router = createBrowserRouter([
  //{ path: "/", element: <Navigate to="/login" /> },
  {
    path: "/",
    element: <App />,
    children: [
      { path: "login", element: <Login /> },
      { path: "register", element: <Register /> },
    ],
  },
]);

ReactDOM.createRoot(document.getElementById("root")!).render(
  <React.StrictMode>
    <AuthProvider>
      <RouterProvider router={router} />
    </AuthProvider>
  </React.StrictMode>
);
