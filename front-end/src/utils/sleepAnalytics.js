function parseDateValue(value) {
  if (!value) {
    return null;
  }

  const stringValue = String(value);
  const dateOnlyMatch = /^([0-9]{4})-([0-9]{2})-([0-9]{2})$/;
  const match = dateOnlyMatch.exec(stringValue);

  if (match) {
    const year = Number(match[1]);
    const month = Number(match[2]) - 1;
    const day = Number(match[3]);
    return new Date(year, month, day);
  }

  const date = new Date(stringValue);
  return Number.isNaN(date.getTime()) ? null : date;
}

export function buildWeeklyBars(records) {
  const week = ["SEG", "TER", "QUA", "QUI", "SEX", "SAB", "DOM"];
  const values = Array(7).fill(null);

  records.forEach((record) => {
    const recordDate = parseDateValue(record.recordDate);
    const duration = Number(record.durationInHours);
    if (
      !recordDate ||
      Number.isNaN(recordDate.getTime()) ||
      !Number.isFinite(duration)
    ) {
      return;
    }

    const dayIndex = (recordDate.getDay() + 6) % 7;
    values[dayIndex] = duration;
  });

  const maxValue = Math.max(
    ...values.filter((value) => Number.isFinite(value)),
    0,
  );
  return week.map((day, index) => {
    const value = values[index];
    const height =
      Number.isFinite(value) && maxValue > 0
        ? Math.max(15, Math.round((value / maxValue) * 100))
        : 15;
    return [day, height, false, value];
  });
}

export function calculateAverageDurationHours(records) {
  const durationValues = records
    .map((record) => Number(record.durationInHours))
    .filter((value) => Number.isFinite(value));

  if (durationValues.length === 0) {
    return null;
  }

  return (
    durationValues.reduce((sum, value) => sum + value, 0) /
    durationValues.length
  );
}

export function calculateGoalCount(records, goalHours = 7) {
  return records.filter((record) => Number(record.durationInHours) >= goalHours)
    .length;
}
