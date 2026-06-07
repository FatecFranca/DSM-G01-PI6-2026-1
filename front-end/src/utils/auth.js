import {
  loginUser as apiLoginUser,
  registerUser as apiRegisterUser,
  extractAccessToken,
  getAuthToken,
  setAuthToken,
  removeAuthToken,
  getUserProfile
} from "../services/api.js";

const AUTH_FLAG_STORAGE_KEY = "sleep-sanctuary-is-authenticated";
const CURRENT_USER_STORAGE_KEY = "sleep-sanctuary-current-user";

function safeRead(key, fallbackValue) {
  if (typeof window === "undefined") {
    return fallbackValue;
  }
  try {
    const rawValue = window.localStorage.getItem(key);
    return rawValue ? JSON.parse(rawValue) : fallbackValue;
  } catch {
    return fallbackValue;
  }
}

function safeWrite(key, value) {
  if (typeof window === "undefined") {
    return;
  }
  window.localStorage.setItem(key, JSON.stringify(value));
}

function clearSession() {
  removeAuthToken();
  if (typeof window !== "undefined") {
    window.localStorage.removeItem(AUTH_FLAG_STORAGE_KEY);
    window.localStorage.removeItem(CURRENT_USER_STORAGE_KEY);
  }
}

function markAuthenticated() {
  safeWrite(AUTH_FLAG_STORAGE_KEY, true);
}

export async function registerUser(userData) {
  if (
    !userData.name ||
    !userData.email ||
    !userData.password ||
    !userData.birthDate ||
    !userData.gender ||
    !userData.profession
  ) {
    return { success: false, message: "Preencha todos os campos obrigatórios para concluir o cadastro." };
  }

  try {
    const parsedHeight = userData.height ? parseFloat(userData.height) : 0;
    const parsedWeight = userData.weight ? parseFloat(userData.weight) : 0;
    const payload = {
      name: userData.name,
      email: userData.email,
      password: userData.password,
      birthDate: userData.birthDate.split("/").reverse().join("-") + "T00:00:00",
      gender: userData.gender,
      heightCm: Number.isFinite(parsedHeight) ? parsedHeight : 0,
      weightKg: Number.isFinite(parsedWeight) ? parsedWeight : 0,
      occupation: userData.profession,
      sleepDisorder: 0
    };

    const response = await apiRegisterUser(payload);
    const accessToken = extractAccessToken(response);

    if (!accessToken) {
      return { success: false, message: "Erro ao registrar usuário." };
    }

    setAuthToken(accessToken);
    markAuthenticated();
    safeWrite(CURRENT_USER_STORAGE_KEY, { name: response.name || userData.name, email: payload.email });

    return {
      success: true,
      user: { name: userData.name, email: userData.email }
    };
  } catch (error) {
    return { success: false, message: error.message || "Erro ao registrar usuário." };
  }
}

export async function loginUser(email, password) {
  try {
    const response = await apiLoginUser(email, password);
    const accessToken = extractAccessToken(response);

    if (!accessToken) {
      return { success: false, message: "Resposta de login inválida." };
    }

    setAuthToken(accessToken);
    markAuthenticated();

    try {
      const profile = await getUserProfile();
      const normalizedProfile = {
        ...profile,
        email,
        profession: profile?.profession || profile?.occupation || "",
        occupation: profile?.occupation || profile?.profession || ""
      };
      safeWrite(CURRENT_USER_STORAGE_KEY, normalizedProfile);
      return { success: true, user: normalizedProfile };
    } catch {
      safeWrite(CURRENT_USER_STORAGE_KEY, { email });
      return { success: true, user: { email } };
    }
  } catch (error) {
    return {
      success: false,
      message: error.message || "Não encontramos uma conta com esse e-mail e senha. Confira os dados e tente novamente."
    };
  }
}

export function logoutUser() {
  clearSession();
}

export function getCurrentUser() {
  return safeRead(CURRENT_USER_STORAGE_KEY, null);
}

export function isAuthenticated() {
  return safeRead(AUTH_FLAG_STORAGE_KEY, false) === true && Boolean(getAuthToken());
}

export function updateCurrentUser(userData) {
  const currentUser = getCurrentUser();
  if (!currentUser) return null;
  const updatedUser = { ...currentUser, ...userData };
  safeWrite(CURRENT_USER_STORAGE_KEY, updatedUser);
  return updatedUser;
}
