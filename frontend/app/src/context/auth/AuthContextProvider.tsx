import { ReactNode, useEffect, useState } from "react";
import AuthContext from "./AuthContext";
import { jwtDecode } from "jwt-decode";

type JwtPayload = {
  username: string;
};

interface AuthProviderProps {
  children: ReactNode;
}

const AuthProvider: React.FC<AuthProviderProps> = ({ children }) => {
  const [token, setToken] = useState<string | null>(null);
  const [username, setUsername] = useState<string | null>(null);

  useEffect(() => {
    const storedToken = localStorage.getItem("jwt");
    if (storedToken) {
      setToken(storedToken);
      const decoded: JwtPayload = jwtDecode<JwtPayload>(storedToken);
      setUsername(decoded.username);
    }
  }, []);

  const login = (newToken: string) => {
    localStorage.setItem("jwt", newToken);
    setToken(newToken);
    const decoded: JwtPayload = jwtDecode<JwtPayload>(newToken);
    setUsername(decoded.username);
  };

  const logout = () => {
    localStorage.removeItem("jwt");
    setToken(null);
    setUsername(null);
  };

  return (
    <AuthContext.Provider value={{ token, username, login, logout }}>
      {children}
    </AuthContext.Provider>
  );
};

export default AuthProvider;
