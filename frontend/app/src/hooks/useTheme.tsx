import { useContext } from "react";
import { ThemeContext, ThemeContextType } from "../context/theme/ThemeContext";

const useTheme = (): ThemeContextType => {
  const context = useContext(ThemeContext);
  if (!context) throw new Error("useTheme must be used within ThemeProvider");
  return context;
};

export default useTheme;
