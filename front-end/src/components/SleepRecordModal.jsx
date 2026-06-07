import { useEffect } from "react";
import MaterialIcon from "./MaterialIcon.jsx";

function formatValue(value, fallback = "Nao informant") {
  if (value === null || value === undefined || value === "") {
    return fallback;
  }

  if (typeof value === "boolean") {
    return value ? "Sim" : "Nao";
  }

  return value;
}

const detailGroups = [
  {
    title: "Resumo do dia",
    items: [
      { icon: "bedtime", label: "Sono", key: "duration" },
      { icon: "star", label: "Qualidade", key: "quality" },
      { icon: "mood", label: "Estresse", key: "stressLevel" },
      { icon: "directions_run", label: "Atividade", key: "activityTime" },
      { icon: "directions_walk", label: "Passos", key: "steps" },
      { icon: "favorite", label: "Pressao arterial", key: "bloodPressure" }
    ]
  },
  {
    title: "Habitos antes de dormir",
    items: [
      { icon: "smartphone", label: "Celular antes de dormir", key: "phoneBeforeSleep" },
      { icon: "coffee", label: "Cafeina", key: "caffeine" },
      { icon: "sports_bar", label: "Alcool", key: "alcohol" }
    ]
  }
];

export default function SleepRecordModal({ isOpen, onClose, record, isLoading = false }) {
  useEffect(() => {
    if (!isOpen) return undefined;

    document.body.classList.add("modal-open");

    const handleKeyDown = (event) => {
      if (event.key === "Escape") {
        onClose();
      }
    };

    window.addEventListener("keydown", handleKeyDown);

    return () => {
      document.body.classList.remove("modal-open");
      window.removeEventListener("keydown", handleKeyDown);
    };
  }, [isOpen, onClose]);

  if (!isOpen || !record) return null;

  const handleBackdropClick = (event) => {
    if (event.target === event.currentTarget) {
      onClose();
    }
  };

  return (
    <div className="profile-settings-modal" role="presentation" onMouseDown={handleBackdropClick}>
      <section
        className="profile-settings-modal__panel sleep-record-modal__panel"
        role="dialog"
        aria-modal="true"
        aria-labelledby="sleep-record-modal-title"
      >
        <header className="profile-settings-modal__header">
          <div>
            <span className="kicker">Historico</span>
            <h2 id="sleep-record-modal-title">Detalhes do registro</h2>
            <p className="sleep-record-modal__date">
              <MaterialIcon>calendar_month</MaterialIcon>
              <span>{formatValue(record.date)}</span>
            </p>
          </div>
          <button className="icon-button" type="button" aria-label="Fechar detalhes do registro" onClick={onClose}>
            <MaterialIcon>close</MaterialIcon>
          </button>
        </header>

        <div className="sleep-record-modal__content">
          <section className="sleep-record-modal__summary card">
            <div className="sleep-record-modal__summary-head">
              <div>
                <span className={`badge badge--${record.tone || "tertiary"}`}>
                  {formatValue(record.goal, "Registro salvo")}
                </span>
                <h3>{formatValue(record.weekday)}</h3>
                {isLoading ? <p>Carregando detalhes completos do registro...</p> : null}
              </div>
              <div className="sleep-record-modal__score">
                <span>Score de sono</span>
                <strong>{formatValue(record.sleepScore || record.score)}</strong>
              </div>
            </div>
            <p>{formatValue(record.note, "Sem observacoes adicionais para este dia.")}</p>
          </section>

          {detailGroups.map((group) => (
            <section className="sleep-record-modal__group" key={group.title}>
              <h3 className="section-title">{group.title}</h3>
              <div className="sleep-record-modal__grid">
                {group.items.map((item) => (
                  <article className="history-stat sleep-record-modal__stat" key={item.key}>
                    <MaterialIcon>{item.icon}</MaterialIcon>
                    <span>{item.label}</span>
                    <strong>{formatValue(record[item.key])}</strong>
                  </article>
                ))}
              </div>
            </section>
          ))}

          <section className="sleep-record-modal__group">
            <h3 className="section-title">Analise final</h3>
            <div className="sleep-record-modal__grid sleep-record-modal__grid--wide">
              <article className="history-stat sleep-record-modal__stat">
                <MaterialIcon>analytics</MaterialIcon>
                <span>Score de sono</span>
                <strong>{formatValue(record.sleepScore || record.score)}</strong>
              </article>
              <article className="history-stat sleep-record-modal__stat">
                <MaterialIcon>neurology</MaterialIcon>
                <span>Disturbio detectado</span>
                <strong>{formatValue(record.detectedDisturbance)}</strong>
              </article>
            </div>
          </section>
        </div>

        <footer className="profile-settings-modal__actions">
          <button className="btn btn--soft" type="button" onClick={onClose}>
            Fechar
          </button>
        </footer>
      </section>
    </div>
  );
}
