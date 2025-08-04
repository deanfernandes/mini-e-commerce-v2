import { StrictMode } from "react";
import { createRoot } from "react-dom/client";
import "./index.css";
import App from "./App";
import "@fortawesome/fontawesome-free/css/all.min.css";
import { BrowserRouter } from "react-router-dom";
import { ThemeProvider } from "./context/theme/ThemeContextProvider";
import AuthProvider from "./context/auth/AuthContextProvider";

createRoot(document.getElementById("root")!).render(
  <AuthProvider>
    <ThemeProvider>
      <BrowserRouter>
        <StrictMode>
          <App />
        </StrictMode>
      </BrowserRouter>
    </ThemeProvider>
  </AuthProvider>
);
