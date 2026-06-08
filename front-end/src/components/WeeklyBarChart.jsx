export default function WeeklyBarChart({ bars = [], compact = false }) {
  const className = compact ? "bar-chart bar-chart--compact" : "bar-chart";

  return (
    <div className={className}>
      {bars.map(([day, height, active, value]) => {
        const hasValue = Number.isFinite(value);
        return (
          <div className={hasValue ? "bar-chart__item active" : "bar-chart__item"} key={day}>
            <span style={{ height: `${height}%` }} />
            <strong>{hasValue ? day : "-"}</strong>
          </div>
        );
      })}
    </div>
  );
}
