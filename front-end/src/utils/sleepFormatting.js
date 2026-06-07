export function formatDate(value) {
  const date = new Date(value);

  if (Number.isNaN(date.getTime())) {
    return value || "Data nao informada";
  }

  return date.toLocaleDateString("pt-BR", {
    day: "2-digit",
    month: "long",
    year: "numeric"
  });
}

export function formatWeekday(value) {
  const date = new Date(value);

  if (Number.isNaN(date.getTime())) {
    return "";
  }

  return date.toLocaleDateString("pt-BR", { weekday: "long" });
}

export function formatDurationFromHours(hoursValue) {
  const hoursNumber = Number(hoursValue);
  if (!Number.isFinite(hoursNumber)) {
    return "Nao informado";
  }

  const hours = Math.floor(hoursNumber);
  const minutes = Math.round((hoursNumber - hours) * 60);
  return `${hours}h ${String(minutes).padStart(2, "0")}m`;
}

export function formatQuality(value) {
  const quality = Number(value);
  if (!Number.isFinite(quality)) {
    return value || "Nao informado";
  }

  if (quality >= 9) return "Excelente";
  if (quality >= 7) return "Boa";
  if (quality >= 5) return "Regular";
  return "Baixa";
}

export function formatBloodPressure(value) {
  if (value === null || value === undefined || value === "") {
    return "Nao informado";
  }

  return value;
}
