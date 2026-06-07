import MaterialIcon from "./MaterialIcon.jsx";

export default function TopBar({ landing = false, onSettingsClick }) {
  const showActions = !landing;

  return (
    <nav className={`top-bar ${landing ? "top-bar--landing" : ""}`}>
      <a className="top-bar__brand" href="#/home">
        Santuário do Sono
      </a>
      {landing && (
        <div className="top-bar__links">
          <a className="active" href="#/onboarding">Home</a>
          <a href="#features">Funcionalidades</a>
          <a href="#insights">Insights</a>
          <a href="#about">Sobre</a>
        </div>
      )}
      {showActions && (
        <div className="top-bar__actions">
        <button className="icon-button" type="button" aria-label="Notificações">
          <MaterialIcon>notifications</MaterialIcon>
        </button>
        <button
          className="icon-button"
          type="button"
          aria-label="Configurações"
          onClick={onSettingsClick}
        >
          <MaterialIcon>settings</MaterialIcon>
        </button>
        </div>
      )}
    </nav>
  );
}
