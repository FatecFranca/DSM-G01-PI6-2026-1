import { useEffect, useState } from "react";
import { MetricCard } from "../components/Card.jsx";
import MaterialIcon from "../components/MaterialIcon.jsx";
import MobileNav from "../components/MobileNav.jsx";
import SideNav from "../components/SideNav.jsx";
import WeeklyBarChart from "../components/WeeklyBarChart.jsx";
import { getInsights, getSleepHistory } from "../services/api.js";
import { buildWeeklyBars, calculateGoalCount } from "../utils/sleepAnalytics.js";
import { formatDurationFromHours } from "../utils/sleepFormatting.js";

function buildQuickInsights(patterns) {
  if (!Array.isArray(patterns) || patterns.length === 0) {
    return [];
  }

  return patterns.map((pattern, index) => ({
    icon: index === 0 ? "psychology" : "schedule",
    title: pattern,
    text: "Padrao identificado a partir dos registros recebidos do backend.",
    tone: index % 2 === 0 ? "primary" : "secondary"
  }));
}

function getRestLevel(score) {
  const value = Number(score);

  if (!Number.isFinite(value)) {
    return "Nao informado";
  }

  if (value >= 80) return "Alto";
  if (value >= 60) return "Moderado";
  return "Baixo";
}

export default function InsightsPage() {
  const [insights, setInsights] = useState(null);
  const [historyRecords, setHistoryRecords] = useState([]);
  const [loading, setLoading] = useState(false);
  const [errorMessage, setErrorMessage] = useState("");

  const displayedRecommendations = Array.isArray(insights?.recommendations) ? insights.recommendations : [];
  const displayedQuickInsights = buildQuickInsights(insights?.patterns);
  const trendBars = buildWeeklyBars(historyRecords);
  const averageSleep = formatDurationFromHours(insights?.averageSleep);
  const averageScore = Number.isFinite(Number(insights?.averageScore))
    ? `${Math.round(Number(insights.averageScore))}%`
    : "Nao informado";
  const nightsInGoal = calculateGoalCount(historyRecords, 7);
  const goalProgress = historyRecords.length > 0 ? Math.round((nightsInGoal / historyRecords.length) * 100) : undefined;
  const restLevel = getRestLevel(insights?.averageScore);
  const validDurations = historyRecords
    .map((record) => Number(record.durationInHours))
    .filter((duration) => Number.isFinite(duration) && duration > 0);
  const bestSleep = validDurations.length > 0 ? Math.max(...validDurations) : null;
  const worstSleep = validDurations.length > 0 ? Math.min(...validDurations) : null;
  const hasHistory = historyRecords.length > 0;

  useEffect(() => {
    loadInsights();
  }, []);

  async function loadInsights() {
    setErrorMessage("");
    setLoading(true);

    try {
      const [data, history] = await Promise.all([getInsights(), getSleepHistory(1, 50)]);
      setInsights(data);
      setHistoryRecords(Array.isArray(history?.items) ? history.items : []);
    } catch (error) {
      console.error("Erro ao buscar insights na API:", error);
      setErrorMessage("Nao foi possivel carregar os insights. Tente novamente em instantes.");
    } finally {
      setLoading(false);
    }
  }

  return (
    <div className="app-shell">
      <SideNav active="insights" />
      <main className="main main--wide">
        <header className="page-header">
          <div>
            <span className="kicker">Analise do Sono</span>
            <h2>Insights</h2>
            <p>Leituras simples sobre sua rotina recente de descanso.</p>
          </div>
          <button
            className="btn btn--primary btn--large"
            type="button"
            onClick={loadInsights}
            disabled={loading}
          >
            <MaterialIcon>{loading ? "autorenew" : "auto_awesome"}</MaterialIcon>
            {loading ? "Atualizando..." : "Atualizar Insights"}
          </button>
        </header>

        {errorMessage && (
          <section className="insights-state insights-state--error">
            <span className="round-icon round-icon--secondary">
              <MaterialIcon>error_outline</MaterialIcon>
            </span>
            <div>
              <h3>Erro ao carregar insights</h3>
              <p>{errorMessage}</p>
            </div>
          </section>
        )}

        <div className="insights-grid">
          <section className="card insight-summary">
            <MaterialIcon className="insight-summary__ghost">nights_stay</MaterialIcon>
            <span className="kicker">Resumo geral</span>
            <h3>{hasHistory ? "Resumo baseado nos registros recebidos" : "Sem dados suficientes"}</h3>
            <p>
              {hasHistory
                ? "Os indicadores abaixo usam exclusivamente os dados reais retornados pelo backend."
                : "Quando houver mais registros no backend, esta tela exibira os insights automaticamente."}
            </p>
            <div className="insight-summary__score">
              <strong>{averageScore}</strong>
              <span>qualidade media</span>
            </div>
          </section>

          <section className="insight-metrics">
            <MetricCard
              icon={<MaterialIcon>bedtime</MaterialIcon>}
              label="Sono medio"
              value={averageSleep}
              detail="Media calculada pelos registros recebidos"
              color="primary"
            />
            <MetricCard
              icon={<MaterialIcon>flag</MaterialIcon>}
              label="Meta atingida"
              value={hasHistory ? `${nightsInGoal}/${historyRecords.length}` : "Nao informado"}
              detail="Noites dentro da meta"
              color="tertiary"
              progress={goalProgress}
            />
            <MetricCard
              icon={<MaterialIcon>bolt</MaterialIcon>}
              label="Nivel de descanso"
              value={restLevel}
              detail="Classificado pela pontuacao media"
              color="secondary"
            />
          </section>

          <section className="card insight-trend">
            <div className="section-heading">
              <div>
                <h3>Evolucao semanal</h3>
                <p>Tendencia de qualidade dos ultimos 7 registros</p>
              </div>
              <span className="badge">{hasHistory ? `${historyRecords.length} registros` : "Sem dados"}</span>
            </div>
            <WeeklyBarChart bars={trendBars} compact />
          </section>

          <section className="card insight-chart">
            <div className="section-heading">
              <div>
                <h3>Dados de sono reais</h3>
                <p>Registros dos ultimos dias usados para gerar os indicadores.</p>
              </div>
              <span className="badge">{hasHistory ? `${historyRecords.length} registros` : "Sem dados"}</span>
            </div>
            {hasHistory ? (
              <>
                <WeeklyBarChart bars={trendBars} />
                <div style={{ display: "grid", gridTemplateColumns: "repeat(2, minmax(0, 1fr))", gap: "16px", marginTop: "24px" }}>
                  <div>
                    <span className="kicker">Maior descanso</span>
                    <h4>{bestSleep ? formatDurationFromHours(bestSleep) : "Nao informado"}</h4>
                  </div>
                  <div>
                    <span className="kicker">Menor descanso</span>
                    <h4>{worstSleep ? formatDurationFromHours(worstSleep) : "Nao informado"}</h4>
                  </div>
                </div>
              </>
            ) : (
              <div className="chart-empty">
                <MaterialIcon>insights</MaterialIcon>
                <p>Nenhum registro de sono suficiente foi encontrado ainda.</p>
              </div>
            )}
          </section>

          <section className="card insight-panel">
            <div className="card-title-row">
              <div>
                <h3>Interpretacoes rapidas</h3>
                <p>O que os dados recentes sugerem</p>
              </div>
              <span className="round-icon round-icon--primary">
                <MaterialIcon>psychology</MaterialIcon>
              </span>
            </div>
            <div className="insight-list">
              {displayedQuickInsights.length > 0 ? displayedQuickInsights.map((item) => (
                <article className="insight-list__item" key={item.title}>
                  <span className={`round-icon round-icon--${item.tone}`}>
                    <MaterialIcon>{item.icon}</MaterialIcon>
                  </span>
                  <div>
                    <h4>{item.title}</h4>
                    <p>{item.text}</p>
                  </div>
                </article>
              )) : <p>Nenhum padrao disponivel no momento.</p>}
            </div>
          </section>

          <section className="card recommendation-card">
            <div className="card-title-row">
              <div>
                <h3>Recomendacoes</h3>
                <p>Acoes simples para testar nos proximos dias</p>
              </div>
              <span className="round-icon round-icon--tertiary">
                <MaterialIcon>tips_and_updates</MaterialIcon>
              </span>
            </div>
            <ul className="recommendation-list">
              {displayedRecommendations.length > 0 ? displayedRecommendations.map((item) => (
                <li key={item}>
                  <MaterialIcon>done</MaterialIcon>
                  <span>{item}</span>
                </li>
              )) : <li><span>Sem recomendacoes disponiveis no momento.</span></li>}
            </ul>
          </section>
        </div>
      </main>
      <MobileNav active="insights" />
    </div>
  );
}