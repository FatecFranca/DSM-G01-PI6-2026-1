import MaterialIcon from "./MaterialIcon.jsx";

export default function MobileNav({ active = "home" }) {
  const items = [
    ["home", "Home", "home", "#/home"],
    ["diario", "Diario", "edit_note", "#/diario"],
    ["insights", "Insights", "insights", "#/insights"],
    ["historico", "Hist.", "history", "#/historico"],
    ["perfil", "Perfil", "person", "#/perfil"]
  ];

  return (
    <nav className="mobile-nav">
      {items.map(([key, label, icon, href]) => (
        <a className={active === key ? "active" : ""} href={href} key={key}>
          <MaterialIcon filled={active === key}>{icon}</MaterialIcon>
          <span>{label}</span>
        </a>
      ))}
    </nav>
  );
}
