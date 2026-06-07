const API_BASE_URL = import.meta.env.VITE_API_BASE_URL ?? "";
const AUTH_TOKEN_STORAGE_KEY = "sleep-sanctuary-auth-token";

const GENDER_TO_API = {
  Feminino: "F",
  Masculino: "M",
  F: "F",
  M: "M"
};

const GENDER_FROM_API = {
  F: "Feminino",
  M: "Masculino"
};

export function getAuthToken() {
  if (typeof window === "undefined") {
    return "";
  }

  const token = window.localStorage.getItem(AUTH_TOKEN_STORAGE_KEY);
  return token ? token.trim() : "";
}

export function extractAccessToken(payload) {
  if (!payload || typeof payload !== "object") {
    return "";
  }

  const directToken = payload.accessToken ?? payload.AccessToken;
  if (typeof directToken === "string" && directToken.trim()) {
    return directToken.trim();
  }

  const nestedToken = payload.tokens ?? payload.Tokens;
  if (nestedToken && typeof nestedToken === "object") {
    const token = nestedToken.accessToken ?? nestedToken.AccessToken;
    if (typeof token === "string" && token.trim()) {
      return token.trim();
    }
  }

  return "";
}

export function setAuthToken(token) {
  if (typeof window !== "undefined" && token) {
    window.localStorage.setItem(AUTH_TOKEN_STORAGE_KEY, token.trim());
  }
}

export function removeAuthToken() {
  if (typeof window !== "undefined") {
    window.localStorage.removeItem(AUTH_TOKEN_STORAGE_KEY);
  }
}

function buildQuery(params = {}) {
  const query = new URLSearchParams();
  Object.entries(params).forEach(([key, value]) => {
    if (value !== undefined && value !== null && value !== "") {
      query.append(key, String(value));
    }
  });

  const queryString = query.toString();
  return queryString ? `?${queryString}` : "";
}

function normalizeBirthDateForApi(birthDate) {
  if (!birthDate) {
    return null;
  }

  const trimmed = String(birthDate).trim();
  const slashMatch = trimmed.match(/^(\d{2})\/(\d{2})\/(\d{4})$/);
  if (slashMatch) {
    const [, day, month, year] = slashMatch;
    return `${year}-${month}-${day}T00:00:00`;
  }

  const isoMatch = trimmed.match(/^(\d{4})-(\d{2})-(\d{2})$/);
  if (isoMatch) {
    return `${trimmed}T00:00:00`;
  }

  return trimmed;
}

function mapGenderToApi(gender) {
  return GENDER_TO_API[gender] || gender;
}

function mapGenderFromApi(gender) {
  return GENDER_FROM_API[gender] || gender;
}

async function parseResponse(response) {
  if (response.status === 204) {
    return null;
  }

  const text = await response.text();
  return text ? JSON.parse(text) : null;
}

function normalizeProfile(profile) {
  if (!profile) {
    return profile;
  }

  const occupation = profile.occupation || profile.profession || "";

  return {
    ...profile,
    birthDate: profile.birthDate
      ? String(profile.birthDate).slice(0, 10).split("-").reverse().join("/")
      : profile.birthDate,
    profession: occupation,
    occupation,
    gender: mapGenderFromApi(profile.gender),
    weight: profile.weight ?? "",
    height: profile.height ?? ""
  };
}

async function request(endpoint, options = {}) {
  const { auth = true, ...fetchOptions } = options;
  const token = auth ? getAuthToken() : "";

  const response = await fetch(`${API_BASE_URL}${endpoint}`, {
    ...fetchOptions,
    headers: {
      "Content-Type": "application/json",
      ...(token ? { Authorization: `Bearer ${token}` } : {}),
      ...fetchOptions.headers
    }
  });

  if (!response.ok) {
    let errorBody;
    try {
      const errorJson = await response.json();
      errorBody = errorJson.errors ? errorJson.errors.join(", ") : JSON.stringify(errorJson);
    } catch {
      errorBody = await response.text();
    }
    throw new Error(errorBody || `Erro ${response.status} ao chamar ${endpoint}`);
  }

  return parseResponse(response);
}

async function requestOrNull(endpoint, options = {}) {
  try {
    return await request(endpoint, options);
  } catch {
    return null;
  }
}

export function loginUser(email, password) {
  return request("/login", {
    method: "POST",
    auth: false,
    body: JSON.stringify({ email, password })
  });
}

export function registerUser(data) {
  return request("/user", {
    method: "POST",
    auth: false,
    body: JSON.stringify(data)
  });
}

export function saveSleepRecord(data) {
  return request("/sleep", {
    method: "POST",
    body: JSON.stringify(data)
  });
}

export function getSleepHistory(pageNumber = 1, pageSize = 20, filters = {}) {
  return request(`/sleep${buildQuery({ pageNumber, pageSize, ...filters })}`);
}

function buildInsightsFallback(historyResponse) {
  const records = Array.isArray(historyResponse?.items) ? historyResponse.items : [];

  if (records.length === 0) {
    return {
      averageSleep: 0,
      averageScore: null,
      patterns: ["Ainda nao ha registros suficientes para gerar padroes."],
      recommendations: ["Preencha mais registros diarios para liberar insights personalizados."]
    };
  }

  const durations = records
    .map((record) => Number(record.durationInHours))
    .filter((value) => Number.isFinite(value) && value > 0);
  const averageSleep = durations.length
    ? durations.reduce((accumulator, value) => accumulator + value, 0) / durations.length
    : 0;

  return {
    averageSleep,
    averageScore: null,
    patterns: [
      `Foram analisados ${records.length} registros recentes.`,
      `Media de sono aproximada: ${averageSleep.toFixed(1)} horas por noite.`
    ],
    recommendations: [
      "Mantenha o registro diario para aumentar a precisao dos insights.",
      "Tente manter horarios de dormir e acordar mais consistentes."
    ]
  };
}

export async function getInsights() {
  const apiInsights = await requestOrNull("/insights");
  if (apiInsights) {
    return apiInsights;
  }

  const historyResponse = await getSleepHistory(1, 50).catch(() => null);
  return buildInsightsFallback(historyResponse);
}

export async function getUserProfile() {
  const profile = await request("/user");
  return normalizeProfile(profile);
}

export async function updateUserProfile(data) {
  const parsedWeight = data.weight === "" || data.weight === undefined ? 0 : parseFloat(data.weight);
  const parsedHeight = data.height === "" || data.height === undefined ? 0 : parseFloat(data.height);
  const birthDate = normalizeBirthDateForApi(data.birthDate);

  const payload = {
    name: data.name,
    birthDate,
    sleepGoal: data.sleepGoal || "",
    occupation: data.profession || data.occupation || "",
    gender: mapGenderToApi(data.gender),
    weight: Number.isFinite(parsedWeight) ? parsedWeight : 0,
    height: Number.isFinite(parsedHeight) ? parsedHeight : 0
  };

  const profile = await request("/user", {
    method: "PUT",
    body: JSON.stringify(payload)
  });

  return normalizeProfile(profile);
}

export async function getSleepRecordById(recordId) {
  return request(`/sleep/${recordId}`);
}

export async function getSleepAnalysisByRecordId(recordId) {
  return requestOrNull(`/sleep/${recordId}/analysis`);
}

export function requestSleepAnalysis(recordId) {
  return request(`/sleep/${recordId}/analysis`, {
    method: "POST"
  });
}
