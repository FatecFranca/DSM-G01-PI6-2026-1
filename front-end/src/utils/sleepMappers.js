import { formatBloodPressure, formatDate, formatQuality, formatWeekday } from "./sleepFormatting.js";

export function mapSleepHistoryRecord(record) {
  const sleepScore = Number(record.sleepScore);
  const recordId = record.sleepRecordId ?? record.id;

  return {
    ...record,
    id: recordId,
    sleepRecordId: recordId,
    date: formatDate(record.recordDate || record.date),
    weekday: formatWeekday(record.recordDate || record.date),
    duration: record.duration || (record.durationInHours ? `${record.durationInHours}h` : "Nao informado"),
    quality: record.quality || formatQuality(record.sleepQuality),
    score: Number.isFinite(sleepScore) ? `${sleepScore}%` : record.score || "Nao informado",
    sleepScore: Number.isFinite(sleepScore) ? String(sleepScore) : record.sleepScore,
    goal: record.goal || (Number.isFinite(sleepScore) && sleepScore >= 75 ? "Meta atingida" : "Em acompanhamento"),
    note: record.note || "Sem observacoes detalhadas para este registro.",
    tone: record.tone || (Number.isFinite(sleepScore) && sleepScore >= 80 ? "tertiary" : "secondary"),
    stressLevel: record.stressLevel ? `${record.stressLevel}/10` : "Nao informado",
    activityTime: record.physicalActivity || "Nao informado",
    steps: record.steps || "Nao informado",
    bloodPressure: formatBloodPressure(record.bloodPressure),
    phoneBeforeSleep: record.screenTime ? record.screenTime > 0 : record.phoneBeforeSleep,
    detectedDisturbance: record.disorder || record.detectedDisturbance || "Nao identificado"
  };
}

export function mapSleepRecordDetails(baseRecord, detailRecord, analysisRecord) {
  if (!detailRecord) {
    return baseRecord;
  }

  const sleepScoreValue = detailRecord.sleepScore ?? analysisRecord?.score;
  const score = Number.isFinite(Number(sleepScoreValue)) ? `${Math.round(Number(sleepScoreValue))}%` : baseRecord.score;

  return {
    ...baseRecord,
    ...detailRecord,
    id: detailRecord.sleepRecordId ?? baseRecord.id,
    sleepRecordId: detailRecord.sleepRecordId ?? baseRecord.sleepRecordId,
    date: formatDate(detailRecord.recordDate || detailRecord.date || baseRecord.date),
    weekday: formatWeekday(detailRecord.recordDate || detailRecord.date || baseRecord.date),
    duration: detailRecord.duration || (detailRecord.durationInHours ? `${detailRecord.durationInHours}h` : baseRecord.duration),
    quality: formatQuality(detailRecord.sleepQuality ?? detailRecord.qualityOfSleep ?? baseRecord.sleepQuality),
    score: score || "Nao informado",
    sleepScore: Number.isFinite(Number(sleepScoreValue)) ? String(Math.round(Number(sleepScoreValue))) : baseRecord.sleepScore,
    stressLevel: Number.isFinite(Number(detailRecord.stressLevel)) ? `${detailRecord.stressLevel}/10` : baseRecord.stressLevel,
    activityTime: Number.isFinite(Number(detailRecord.physicalActivityMinutes))
      ? `${detailRecord.physicalActivityMinutes} min`
      : baseRecord.activityTime,
    steps: detailRecord.dailySteps ?? baseRecord.steps,
    bloodPressure: formatBloodPressure(detailRecord.bloodPressure ?? baseRecord.bloodPressure),
    phoneBeforeSleep: detailRecord.screenBeforeSleep ?? baseRecord.phoneBeforeSleep,
    caffeine: detailRecord.caffeine ?? baseRecord.caffeine,
    alcohol: detailRecord.alcohol ?? baseRecord.alcohol,
    detectedDisturbance: analysisRecord?.problem || detailRecord.disorder || baseRecord.detectedDisturbance,
    note: detailRecord.notes || baseRecord.note
  };
}
