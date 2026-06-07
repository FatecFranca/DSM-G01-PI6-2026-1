import { useEffect, useState } from "react";
import { ActionCard, MetricCard } from "../components/Card.jsx";
import MaterialIcon from "../components/MaterialIcon.jsx";
import MobileNav from "../components/MobileNav.jsx";
import ProfileSettingsModal from "../components/ProfileSettingsModal.jsx";
import SideNav from "../components/SideNav.jsx";
import TopBar from "../components/TopBar.jsx";
import { getUserProfile, updateUserProfile } from "../services/api.js";
import { calculateBmi, formatBmi, getBmiClassification } from "../utils/health.js";
import { getStoredProfile, saveStoredProfile } from "../utils/storage.js";

const journeyImage =
  "https://lh3.googleusercontent.com/aida-public/AB6AXuBxMAUAzcLOEPT8u4syvcc_Urn-ojW5qjxLDGtc1guev_PuK0p8G6rDezGdRWjp5KgWuASbHCE3e38zwTQwJ36YA-SofE8ehUponirfBSBw2psUWsOsB7iv4PH3KQ-gvIzLGiR9WSNvuge6jlSTBPopJqieUXsqqMpjN3eL9t65r4l2gHZwc-Ph8tSg0uJcFkgxl88JNuHPzCDsnEuKPTTCuawMqLJ00GleXQ8I9D4R4KY8Hj9zmIlhPF7K48Eh7TWrhYEo5rW5ff0";

function parseBirthDate(birthDate) {
  if (!birthDate) return null;

  const isoMatch = birthDate.trim().match(/^(\d{4})-(\d{2})-(\d{2})$/);
  if (isoMatch) {
    const [, yearValue, monthValue, dayValue] = isoMatch;
    const date = new Date(Number(yearValue), Number(monthValue) - 1, Number(dayValue));
    return Number.isNaN(date.getTime()) ? null : date;
  }

  const match = birthDate.trim().match(/^(\d{2})\/(\d{2})\/(\d{4})$/);
  if (!match) return null;

  const [, dayValue, monthValue, yearValue] = match;
  const day = Number(dayValue);
  const month = Number(monthValue);
  const year = Number(yearValue);
  const date = new Date(year, month - 1, day);

  const isValidDate =
    date.getFullYear() === year &&
    date.getMonth() === month - 1 &&
    date.getDate() === day;

  return isValidDate ? date : null;
}

function calculateAge(birthDate, currentDate = new Date()) {
  const parsedBirthDate = parseBirthDate(birthDate);
  if (!parsedBirthDate || parsedBirthDate > currentDate) return null;

  let age = currentDate.getFullYear() - parsedBirthDate.getFullYear();
  const birthdayThisYear = new Date(
    currentDate.getFullYear(),
    parsedBirthDate.getMonth(),
    parsedBirthDate.getDate()
  );

  if (currentDate < birthdayThisYear) {
    age -= 1;
  }

  return age;
}

export default function ProfilePage({ currentTheme = "light", onThemeChange }) {
  const [isSettingsOpen, setIsSettingsOpen] = useState(false);
  const [profileSettings, setProfileSettings] = useState(getStoredProfile);
  const modalSettings = { ...profileSettings, theme: currentTheme };
  const calculatedAge = calculateAge(profileSettings.birthDate);
  const ageValue = calculatedAge === null ? "Nao informado" : `${calculatedAge} anos`;
  const ageDetail = calculatedAge === null
    ? "Atualize a data de nascimento"
    : "Calculada pela data de nascimento";
  const bmi = calculateBmi(profileSettings.weight, profileSettings.height);
  const bmiValue = formatBmi(bmi);
  const bmiClassification = getBmiClassification(bmi) || "Aguardando dados corporais";
  const bodyMetrics = [
    profileSettings.weight ? `${profileSettings.weight} kg` : null,
    profileSettings.height ? `${profileSettings.height} cm` : null
  ].filter(Boolean).join(" • ") || "Complete peso e altura";

  const displayName = profileSettings.name || "Usuario";
  const profession = profileSettings.profession || "Cargo nao informado";
  const gender = profileSettings.gender || "Genero nao informado";
  const sleepGoal = profileSettings.sleepGoal || "Meta nao definida";

  useEffect(() => {
    let isMounted = true;

    async function loadProfile() {
      try {
        const userProfile = await getUserProfile();
        if (isMounted && userProfile) {
          const savedProfile = saveStoredProfile(userProfile);
          setProfileSettings(savedProfile || userProfile);
        }
      } catch (error) {
        console.error("Erro ao buscar perfil na API:", error);
      }
    }

    loadProfile();

    return () => {
      isMounted = false;
    };
  }, []);

  const handleSettingsSave = async (settings) => {
    const { theme, ...profileData } = settings;
    onThemeChange?.(theme);

    try {
      const updatedProfile = await updateUserProfile(profileData);
      const savedProfile = saveStoredProfile(updatedProfile);
      setProfileSettings(savedProfile);
    } catch (error) {
      console.error("Erro ao salvar perfil na API:", error);
      const savedProfile = saveStoredProfile(profileData);
      setProfileSettings(savedProfile);
    }
  };

  return (
    <div className="app-shell">
      <SideNav active="perfil" />
      <TopBar onSettingsClick={() => setIsSettingsOpen(true)} />
      <main className="main main--topbar">
        <section className="profile-hero">
          <div className="profile-hero__content">
            <span className="kicker">Perfil</span>
            <p className="profile-hero__greeting">Ola, {displayName}</p>
            <h1>{displayName}</h1>
            <p className="profile-hero__subtitle"><MaterialIcon>work</MaterialIcon>{profession}</p>
            <div className="profile-hero__badges" aria-label="Dados principais do perfil">
              <span className="badge"><MaterialIcon>wc</MaterialIcon>{gender}</span>
              <span className="badge"><MaterialIcon>bedtime</MaterialIcon>Meta: {sleepGoal}</span>
              <span className="badge"><MaterialIcon>cake</MaterialIcon>{ageValue}</span>
            </div>
          </div>
        </section>

        <section className="profile-metrics">
          <MetricCard icon={<MaterialIcon>bedtime</MaterialIcon>} label="Meta de Sono" value={profileSettings.sleepGoal} progress={85} />
          <MetricCard icon={<MaterialIcon>cake</MaterialIcon>} label="Idade" value={ageValue} detail={ageDetail} color="secondary" />
          <MetricCard icon={<MaterialIcon>analytics</MaterialIcon>} label="Qualidade Media" value="82%" detail="+5% em relacao ao mes anterior" color="tertiary" />
          <MetricCard icon={<MaterialIcon>monitor_weight</MaterialIcon>} label="IMC" value={bmiValue} detail={bmiClassification} color="primary" />
          <MetricCard icon={<MaterialIcon>fitness_center</MaterialIcon>} label="Peso e Altura" value={bodyMetrics} detail="Base para o calculo automatico do IMC" color="secondary" />
          <MetricCard icon={<MaterialIcon>person</MaterialIcon>} label="Genero" value={profileSettings.gender || "Nao informado"} detail="Informacao editavel nas configuracoes" color="tertiary" />
        </section>

        <div className="profile-grid">
          <div className="stack">
            <section>
              <h3 className="section-title">Preferencias de Sono</h3>
              <div className="stack stack--small">
                <ActionCard icon={<MaterialIcon>schedule</MaterialIcon>} title="Horario de Dormir" subtitle="Ideal: 22:30" action="Alterar" />
                <ActionCard icon={<MaterialIcon>volume_up</MaterialIcon>} title="Sons Relaxantes" subtitle="Chuva Suave, Ruido Branco" action="Gerenciar" tone="secondary" />
                <ActionCard icon={<MaterialIcon>vibration</MaterialIcon>} title="Despertar Inteligente" subtitle="Ativado (Janela de 30min)" tone="tertiary" toggle />
              </div>
            </section>

            <section>
              <h3 className="section-title">Configuracoes de Conta</h3>
              <div className="link-list">
                {["Informacoes Pessoais", "Privacidade e Seguranca", "Notificacoes por E-mail"].map((item) => (
                  <a href="#" key={item}>
                    <span>{item}</span>
                    <MaterialIcon>chevron_right</MaterialIcon>
                  </a>
                ))}
              </div>
            </section>
          </div>

          <div className="stack">
            <section className="journey-card">
              <img src={journeyImage} alt="Ceu noturno abstrato" />
              <div>
                <h4>Explore a Jornada do Descanso</h4>
                <p>Sua dedicacao ao sono de qualidade esta transformando seu bem-estar. {displayName}, voce ja completou 14 noites seguidas de sono profundo.</p>
                <button className="btn btn--light" type="button">Ver Conquistas</button>
              </div>
            </section>

            <section className="support-card">
              <h3 className="section-title">Resumo Corporal</h3>
              <div className="card profile-summary-card">
                <div className="profile-summary-card__row">
                  <span>IMC</span>
                  <strong>{bmiValue}</strong>
                </div>
                <div className="profile-summary-card__row">
                  <span>Classificacao</span>
                  <strong>{bmiClassification}</strong>
                </div>
                <div className="profile-summary-card__row">
                  <span>Peso</span>
                  <strong>{profileSettings.weight ? `${profileSettings.weight} kg` : "Nao informado"}</strong>
                </div>
                <div className="profile-summary-card__row">
                  <span>Altura</span>
                  <strong>{profileSettings.height ? `${profileSettings.height} cm` : "Nao informado"}</strong>
                </div>
              </div>
            </section>
          </div>
        </div>

        <footer className="page-footer">Santuario do Sono © 2024 • Feito para o seu descanso</footer>
      </main>
      <ProfileSettingsModal
        isOpen={isSettingsOpen}
        initialData={modalSettings}
        onClose={() => setIsSettingsOpen(false)}
        onSave={handleSettingsSave}
      />
      <MobileNav active="perfil" />
    </div>
  );
}
