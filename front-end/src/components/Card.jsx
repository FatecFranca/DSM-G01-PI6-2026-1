export function MetricCard({ icon, label, value, detail, color = "primary", progress }) {
  return (
    <div className="metric-card">
      <div>
        <div className={`eyebrow eyebrow--${color}`}>
          {icon}
          <span>{label}</span>
        </div>
        <strong>{value}</strong>
      </div>
      {progress !== undefined ? (
        <div className="progress">
          <span style={{ width: `${progress}%` }} />
        </div>
      ) : (
        <p className={`metric-card__detail text-${color}`}>{detail}</p>
      )}
    </div>
  );
}

export function ActionCard({ icon, title, subtitle, action, tone = "primary", toggle = false }) {
  return (
    <div className="action-card">
      <div className="action-card__body">
        <div className={`round-icon round-icon--${tone}`}>{icon}</div>
        <div>
          <h4>{title}</h4>
          <p>{subtitle}</p>
        </div>
      </div>
      {toggle ? <span className="switch switch--on" aria-hidden="true" /> : <button type="button">{action}</button>}
    </div>
  );
}
