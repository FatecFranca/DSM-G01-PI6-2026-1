import { useEffect, useMemo, useState } from "react";
import DashboardPage from "./pages/DashboardPage.jsx";
import DiaryEntryPage from "./pages/DiaryEntryPage.jsx";
import HistoryPage from "./pages/HistoryPage.jsx";
import InsightsPage from "./pages/InsightsPage.jsx";
import LoginPage from "./pages/LoginPage.jsx";
import OnboardingPage from "./pages/OnboardingPage.jsx";
import ProfilePage from "./pages/ProfilePage.jsx";
import RegisterPage from "./pages/RegisterPage.jsx";
import useAppTheme from "./hooks/useAppTheme.js";
import { isAuthenticated } from "./utils/auth.js";

const pages = {
  onboarding: OnboardingPage,
  login: LoginPage,
  cadastro: RegisterPage,
  home: DashboardPage,
  diario: DiaryEntryPage,
  insights: InsightsPage,
  historico: HistoryPage,
  perfil: ProfilePage
};

const publicPages = new Set(["onboarding", "login", "cadastro"]);

function getCurrentPage() {
  const hashRoute = window.location.hash.replace("#/", "");
  const pathRoute = window.location.pathname.replace("/", "");

  return hashRoute || pathRoute || "home";
}

export default function App() {
  const [currentPage, setCurrentPage] = useState(getCurrentPage);
  const [theme, setTheme] = useAppTheme();

  useEffect(() => {
    const syncRoute = () => setCurrentPage(getCurrentPage());

    window.addEventListener("hashchange", syncRoute);
    window.addEventListener("popstate", syncRoute);
    window.addEventListener("storage", syncRoute);

    return () => {
      window.removeEventListener("hashchange", syncRoute);
      window.removeEventListener("popstate", syncRoute);
      window.removeEventListener("storage", syncRoute);
    };
  }, []);

  useEffect(() => {
    const authenticated = isAuthenticated();

    if (!authenticated && !publicPages.has(currentPage)) {
      window.location.hash = "/login";
      return;
    }

    if (authenticated && (currentPage === "login" || currentPage === "cadastro")) {
      window.location.hash = "/home";
    }
  }, [currentPage]);

  const resolvedPage = useMemo(() => {
    const authenticated = isAuthenticated();

    if (!authenticated && !publicPages.has(currentPage)) {
      return "login";
    }

    if (authenticated && (currentPage === "login" || currentPage === "cadastro")) {
      return "home";
    }

    return pages[currentPage] ? currentPage : "home";
  }, [currentPage]);

  const Page = pages[resolvedPage] || DashboardPage;

  return <Page currentTheme={theme} onThemeChange={setTheme} />;
}
