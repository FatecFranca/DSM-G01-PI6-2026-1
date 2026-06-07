import { useLayoutEffect, useState } from "react";

const THEME_STORAGE_KEY = "app-theme";
const validThemes = ["light", "dark"];

function getStoredTheme() {
  const storedTheme = window.localStorage.getItem(THEME_STORAGE_KEY);
  return validThemes.includes(storedTheme) ? storedTheme : "light";
}

export default function useAppTheme() {
  const [theme, setTheme] = useState(getStoredTheme);

  useLayoutEffect(() => {
    document.documentElement.dataset.theme = theme;
    document.documentElement.classList.toggle("dark", theme === "dark");
    document.documentElement.classList.toggle("light", theme === "light");
    window.localStorage.setItem(THEME_STORAGE_KEY, theme);
  }, [theme]);

  return [theme, setTheme];
}
