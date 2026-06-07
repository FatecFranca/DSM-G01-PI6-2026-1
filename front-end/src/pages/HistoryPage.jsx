import { useEffect, useState } from "react";
import { MetricCard } from "../components/Card.jsx";
import MaterialIcon from "../components/MaterialIcon.jsx";
import MobileNav from "../components/MobileNav.jsx";
import SideNav from "../components/SideNav.jsx";
import SleepRecordModal from "../components/SleepRecordModal.jsx";
import { getSleepAnalysisByRecordId, getSleepHistory, getSleepRecordById } from "../services/api.js";
import { calculateAverageDurationHours, calculateGoalCount } from "../utils/sleepAnalytics.js";
import { formatDurationFromHours } from "../utils/sleepFormatting.js";
import { mapSleepHistoryRecord, mapSleepRecordDetails } from "../utils/sleepMappers.js";
import { getStoredDiaryEntries } from "../utils/storage.js";

const filters = ["7 dias", "30 dias", "Este mes"];

function getHistoryDateRange(filterLabel) {
  const end = new Date();
  const start = new Date();

  if (filterLabel === "Este mes") {
    start.setDate(1);
  } else {
    const days = filterLabel === "30 dias" ? 30 : 7;
    start.setDate(end.getDate() - days);
  }

  return {
    sleepStart: start.toISOString().slice(0, 10),
    sleepEnd: end.toISOString().slice(0, 10)
  };
}

export default function HistoryPage() {
  const [activeFilter, setActiveFilter] = useState(filters[0]);
  const [selectedRecord, setSelectedRecord] = useState(null);
  const [isLoadingDetails, setIsLoadingDetails] = useState(false);
  const [records, setRecords] = useState(() => {
    const storedEntries = getStoredDiaryEntries();
    return storedEntries.length > 0 ? storedEntries : [];
  });
  const averageDurationHours = calculateAverageDurationHours(records);
  const averageDurationText = Number.isFinite(averageDurationHours)
    ? formatDurationFromHours(averageDurationHours)
    : "Nao informado";
  const goalCount = calculateGoalCount(records, 7);

  useEffect(() => {
    let isMounted = true;

    async function loadHistory() {
      try {
        const dateRange = getHistoryDateRange(activeFilter);
        const response = await getSleepHistory(1, 50, dateRange);
        const history = response?.items || response || [];
        if (isMounted && Array.isArray(history)) {
          setRecords(history.map(mapSleepHistoryRecord));
        }
      } catch (error) {
        console.error("Erro ao buscar historico na API:", error);
      }
    }

    loadHistory();

    return () => {
      isMounted = false;
    };
  }, [activeFilter]);

  const handleOpenRecord = async (record) => {
    setSelectedRecord(record);
    const recordId = record.sleepRecordId || record.id;
    if (!recordId) {
      return;
    }

    setIsLoadingDetails(true);
    try {
      const [detailRecord, analysisRecord] = await Promise.all([
        getSleepRecordById(recordId).catch(() => null),
        getSleepAnalysisByRecordId(recordId).catch(() => null)
      ]);

      if (!detailRecord && !analysisRecord) {
        return;
      }

      setSelectedRecord((currentRecord) => mapSleepRecordDetails(currentRecord || record, detailRecord, analysisRecord));
    } finally {
      setIsLoadingDetails(false);
    }
  };

  const handleCloseRecord = () => {
    setSelectedRecord(null);
  };

  return (
    <div className="app-shell">
      <SideNav active="historico" />
      <main className="main main--narrow">
        <header className="page-header">
          <div>
            <span className="kicker">Acompanhamento</span>
            <h2>Historico</h2>
            <p>Seus registros anteriores de sono organizados por data.</p>
          </div>
          <div className="history-filters" aria-label="Filtros do historico">
            {filters.map((filter) => (
              <button
                className={filter === activeFilter ? "active" : ""}
                type="button"
                key={filter}
                onClick={() => setActiveFilter(filter)}
              >
                {filter}
              </button>
            ))}
          </div>
        </header>

        <section className="history-summary">
          <MetricCard
            icon={<MaterialIcon>calendar_month</MaterialIcon>}
            label="Registros"
            value={String(records.length)}
            detail="Ultimos 30 dias"
            color="primary"
          />
          <MetricCard
            icon={<MaterialIcon>bedtime</MaterialIcon>}
            label="Media recente"
            value={averageDurationText}
            detail="Sono por noite"
            color="secondary"
          />
          <MetricCard
            icon={<MaterialIcon>flag</MaterialIcon>}
            label="Metas"
            value={records.length > 0 ? String(goalCount) : "Nao informado"}
            detail="Noites dentro da meta"
            color="tertiary"
          />
        </section>

        <section className="card history-panel">
          <div className="card-title-row">
            <div>
              <h3>Registros recentes</h3>
              <p>Lista cronologica dos ultimos dias acompanhados</p>
            </div>
            <span className="round-icon round-icon--primary">
              <MaterialIcon>history</MaterialIcon>
            </span>
          </div>

          <div className="history-list">
            {records.map((record) => (
              <article
                className="history-record"
                key={record.id || record.createdAt || record.date}
                role="button"
                tabIndex={0}
                onClick={() => handleOpenRecord(record)}
                onKeyDown={(event) => {
                  if (event.key === "Enter" || event.key === " ") {
                    event.preventDefault();
                    handleOpenRecord(record);
                  }
                }}
                aria-label={`Abrir detalhes do registro de ${record.weekday}, ${record.date}`}
              >
                <div className="history-record__date">
                  <strong>{record.weekday}</strong>
                  <span>{record.date}</span>
                </div>
                <div className="history-record__metrics">
                  <HistoryStat icon="schedule" label="Duracao" value={record.duration} />
                  <HistoryStat icon="star" label="Qualidade" value={record.quality} />
                  <HistoryStat icon="analytics" label="Score" value={record.score} />
                </div>
                <div className="history-record__note">
                  <span className={`badge badge--${record.tone}`}>{record.goal}</span>
                  <p>{record.note}</p>
                </div>
              </article>
            ))}
          </div>
        </section>

        <section className="history-week-card">
          <div>
            <span className="round-icon round-icon--secondary">
              <MaterialIcon>date_range</MaterialIcon>
            </span>
            <div>
              <h3>Semana atual</h3>
              <p>
                {records.length > 0
                  ? `Media de ${averageDurationText} por noite considerando os registros carregados.`
                  : "Sem dados suficientes para resumir a semana atual."}
              </p>
            </div>
          </div>
          <button className="btn btn--soft" type="button" disabled={records.length === 0}>Ver semana completa</button>
        </section>
      </main>
      <SleepRecordModal
        isOpen={selectedRecord !== null}
        record={selectedRecord}
        isLoading={isLoadingDetails}
        onClose={handleCloseRecord}
      />
      <MobileNav active="historico" />
    </div>
  );
}

function HistoryStat({ icon, label, value }) {
  return (
    <div className="history-stat">
      <MaterialIcon>{icon}</MaterialIcon>
      <span>{label}</span>
      <strong>{value}</strong>
    </div>
  );
}
