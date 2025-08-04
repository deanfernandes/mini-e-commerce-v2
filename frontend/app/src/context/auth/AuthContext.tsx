import { createContext } from "react";

export type AuthContextType = {
  token: string | null;
  username: string | null;
  login: (token: string) => void;
  logout: () => void;
};

const AuthContext = createContext<AuthContextType | undefined>(undefined);

export default AuthContext;
