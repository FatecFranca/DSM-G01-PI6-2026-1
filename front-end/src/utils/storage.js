import { getCurrentUser, updateCurrentUser } from "./auth.js";

const DIARY_STORAGE_KEY = "sleep-sanctuary-diary-entries";

export const defaultUserProfile = {
  name: "",
  email: "",
  birthDate: "",
  sleepGoal: "",
  profession: "",
  gender: "Prefiro nao informar",
  weight: "",
  height: ""
};

function safeRead(key, fallbackValue) {
  if (typeof window === "undefined") {
    return fallbackValue;
  }

  try {
    const rawValue = window.localStorage.getItem(key);

    if (!rawValue) {
      return fallbackValue;
    }

    return JSON.parse(rawValue);
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

function getDiaryEntriesMap() {
  const entries = safeRead(DIARY_STORAGE_KEY, {});
  return entries && typeof entries === "object" && !Array.isArray(entries) ? entries : {};
}

export function getStoredProfile() {
  return {
    ...defaultUserProfile,
    ...(getCurrentUser() || {})
  };
}

export function saveStoredProfile(profileData) {
  const nextProfile = {
    ...defaultUserProfile,
    ...getStoredProfile(),
    ...profileData
  };

  return updateCurrentUser(nextProfile) || nextProfile;
}

export function getStoredDiaryEntries() {
  const currentUser = getCurrentUser();

  if (!currentUser?.email) {
    return [];
  }

  const diaryEntriesMap = getDiaryEntriesMap();
  const entries = diaryEntriesMap[currentUser.email];
  return Array.isArray(entries) ? entries : [];
}

export function saveDiaryEntry(entryData) {
  const currentUser = getCurrentUser();

  if (!currentUser?.email) {
    return [];
  }

  const diaryEntriesMap = getDiaryEntriesMap();
  const currentEntries = Array.isArray(diaryEntriesMap[currentUser.email])
    ? diaryEntriesMap[currentUser.email]
    : [];
  const nextEntries = [entryData, ...currentEntries].slice(0, 30);

  safeWrite(DIARY_STORAGE_KEY, {
    ...diaryEntriesMap,
    [currentUser.email]: nextEntries
  });

  return nextEntries;
}
