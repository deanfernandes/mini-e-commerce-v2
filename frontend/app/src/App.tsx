import Footer from "./components/Footer";
import Header from "./components/Header";
import { Routes, Route, Navigate } from "react-router-dom";
import Home from "./pages/Home";
import Login from "./pages/Login";
import NotFound from "./pages/NotFound";
import SignUp from "./pages/SignUp";
import { useAuth } from "./hooks/useAuth";
import MainLayout from "./layout/MainLayout";
import Products from "./pages/Products";
import ProductDetails from "./pages/ProductDetails";

export default function App() {
  const { token } = useAuth();

  return (
    <div className="min-h-screen flex flex-col">
      <Header />
      <Routes>
        <Route path="/" element={<MainLayout token={token} />}>
          <Route index element={<Home />} />
          <Route
            path="signup"
            element={!token ? <SignUp /> : <Navigate to="/" replace />}
          />
          <Route
            path="login"
            element={!token ? <Login /> : <Navigate to="/" replace />}
          />
          <Route
            path="products"
            element={token ? <Products /> : <Navigate to="/login" replace />}
          />
          <Route path="products/:id" element={<ProductDetails />} />
          <Route path="*" element={<NotFound />} />
        </Route>
      </Routes>
      <Footer />
    </div>
  );
}
