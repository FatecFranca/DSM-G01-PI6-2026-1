export default function WeeklyBarChart({ bars = [], compact = false }) {
  const className = compact ? "bar-chart bar-chart--compact" : "bar-chart";

  return (
    <div className={className}>
      {bars.map(([day, height, active, value]) => (
        <div className={active ? "bar-chart__item active" : "bar-chart__item"} key={day}>
          <span style={{ height: `${height}%` }} />
          <strong>{value ? day : "-"}</strong>
        </div>
      ))}
    </div>
  );
}
