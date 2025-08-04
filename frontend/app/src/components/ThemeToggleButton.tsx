import React from "react";
import { ThemeContext } from "../context/theme/ThemeContext";
import useTheme from "../hooks/useTheme";

export const ThemeToggleButton: React.FC = () => {
  const { theme, toggleTheme } = useTheme();

  return (
    <button
      onClick={toggleTheme}
      className="rounded-full w-10 h-10 bg-gray-200 dark:bg-gray-700 text-xl cursor-pointer"
      title={theme === "dark" ? "Dark" : "Light"}
    >
      {theme === "light" ? "🌙" : "☀️"}
    </button>
  );
};
