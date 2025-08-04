import Footer from "./components/Footer";
import Header from "./components/Header";
import { Routes, Route, Navigate } from "react-router-dom";
import Home from "./pages/Home";
import Login from "./pages/Login";
import NotFound from "./pages/NotFound";
import SignUp from "./pages/SignUp";
import { useAuth } from "./hooks/useAuth";

export default function App() {
  const { token } = useAuth();

  return (
    <div className="min-h-screen flex flex-col">
      <Header />
      <main className="flex-grow flex flex-col">
        <Routes>
          <Route path="/" element={<Home />} />
          <Route
            path="/signup"
            element={!token ? <SignUp /> : <Navigate to="/" replace />}
          />
          <Route
            path="/login"
            element={!token ? <Login /> : <Navigate to="/" replace />}
          />
          <Route path="*" element={<NotFound />} />
        </Routes>
      </main>
      <Footer />
    </div>
  );
}
