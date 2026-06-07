import MaterialIcon from "./MaterialIcon.jsx";
import { logoutUser } from "../utils/auth.js";

const navItems = [
  { key: "home", label: "Home", icon: "home", href: "#/home" },
  { key: "diario", label: "Diario", icon: "edit_note", href: "#/diario" },
  { key: "insights", label: "Insights", icon: "insights", href: "#/insights" },
  { key: "historico", label: "Historico", icon: "history", href: "#/historico" },
  { key: "perfil", label: "Perfil", icon: "person", href: "#/perfil" }
];

export default function SideNav({ active = "home" }) {
  const handleLogout = (event) => {
    event.preventDefault();
    logoutUser();
    window.location.hash = "/login";
  };

  return (
    <aside className="side-nav">
      <div className="side-nav__brand">
        <h1>Santuario</h1>
        <p>Menu Principal</p>
      </div>

      <nav className="side-nav__links" aria-label="Menu principal">
        {navItems.map((item) =>
          item.key === active ? (
            <div className="side-nav__item side-nav__item--active" key={item.key}>
              <MaterialIcon filled>{item.icon}</MaterialIcon>
              <span>{item.label}</span>
            </div>
          ) : (
            <a className="side-nav__item" href={item.href} key={item.key}>
              <MaterialIcon>{item.icon}</MaterialIcon>
              <span>{item.label}</span>
            </a>
          )
        )}
      </nav>

      <div className="side-nav__footer">
        <div className="side-nav__support">
          <a href="#">
            <MaterialIcon>help</MaterialIcon>
            Suporte
          </a>
          <a className="danger" href="#/login" onClick={handleLogout}>
            <MaterialIcon>logout</MaterialIcon>
            Sair
          </a>
        </div>
      </div>
    </aside>
  );
}
