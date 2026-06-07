function parseTimeToMinutes(timeValue) {
  if (!timeValue || typeof timeValue !== "string") {
    return null;
  }

  const match = timeValue.match(/^(\d{2}):(\d{2})$/);
  if (!match) {
    return null;
  }

  const hours = Number(match[1]);
  const minutes = Number(match[2]);

  if (
    !Number.isInteger(hours) ||
    !Number.isInteger(minutes) ||
    hours < 0 ||
    hours > 23 ||
    minutes < 0 ||
    minutes > 59
  ) {
    return null;
  }

  return (hours * 60) + minutes;
}

export function calculateSleepDuration(sleepTime, wakeTime) {
  const sleepMinutes = parseTimeToMinutes(sleepTime);
  const wakeMinutes = parseTimeToMinutes(wakeTime);

  if (sleepMinutes === null || wakeMinutes === null) {
    return null;
  }

  let durationInMinutes = wakeMinutes - sleepMinutes;

  if (durationInMinutes < 0) {
    durationInMinutes += 24 * 60;
  }

  const hours = Math.floor(durationInMinutes / 60);
  const minutes = durationInMinutes % 60;

  return {
    totalMinutes: durationInMinutes,
    hours,
    minutes,
    formatted: `${hours}h ${String(minutes).padStart(2, "0")}m`,
    label: `${hours}h ${minutes}min`
  };
}
