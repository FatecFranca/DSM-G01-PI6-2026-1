import { useMemo, useState } from "react";
import MaterialIcon from "../components/MaterialIcon.jsx";
import MobileNav from "../components/MobileNav.jsx";
import SideNav from "../components/SideNav.jsx";
import { saveSleepRecord } from "../services/api.js";
import { calculateSleepDuration } from "../utils/sleep.js";
import { saveDiaryEntry } from "../utils/storage.js";

const initialDiaryState = {
  physicalActivity: "Medio",
  sleepTime: "22:30",
  wakeTime: "06:30",
  stressLevel: 5,
  sleepQuality: 8,
  mentalFatigue: 4,
  heartRate: "",
  bloodPressure: "",
  dailySteps: "",
  screenTimeBeforeSleep: "",
  habits: {
    caffeine: false,
    screens: true,
    alcohol: false
  }
};

const activityOptions = ["Baixo", "Medio", "Alto"];
const activityApiValues = {
  Baixo: 15,
  Medio: 30,
  Alto: 60
};

function getCurrentDateParts() {
  const today = new Date();
  const weekday = today.toLocaleDateString("pt-BR", { weekday: "long" });
  const fullDate = today.toLocaleDateString("pt-BR", {
    day: "2-digit",
    month: "long",
    year: "numeric"
  });

  return {
    weekday: weekday.charAt(0).toUpperCase() + weekday.slice(1),
    fullDate
  };
}

function getSleepQualityLabel(value) {
  if (value <= 3) return "Noite instavel";
  if (value <= 6) return "Sono moderado";
  if (value <= 8) return "Descanso consistente";
  return "Descanso profundo";
}

export default function DiaryEntryPage() {
  const [formData, setFormData] = useState(initialDiaryState);
  const [saveMessage, setSaveMessage] = useState("");
  const [isSaving, setIsSaving] = useState(false);
  const dateParts = getCurrentDateParts();
  const calculatedDuration = useMemo(
    () => calculateSleepDuration(formData.sleepTime, formData.wakeTime),
    [formData.sleepTime, formData.wakeTime]
  );

  const handleFieldChange = (event) => {
    const { name, value } = event.target;

    if (name === "bloodPressure") {
      if (!/^\d*$/.test(value)) return;

      setFormData((currentData) => ({
        ...currentData,
        bloodPressure: value === "" ? "" : Number(value)
      }));
      return;
    }

    setFormData((currentData) => ({ ...currentData, [name]: value }));
  };

  const handleHabitChange = (event) => {
    const { name, checked } = event.target;
    setFormData((currentData) => ({
      ...currentData,
      habits: {
        ...currentData.habits,
        [name]: checked
      }
    }));
  };

  const buildApiPayload = () => {
    const now = new Date();
    const [wakeH, wakeM] = formData.wakeTime.split(':').map(Number);
    const [sleepH, sleepM] = formData.sleepTime.split(':').map(Number);
    
    let sleepDate = new Date(now);
    sleepDate.setHours(sleepH, sleepM, 0, 0);
    
    let wakeDate = new Date(now);
    wakeDate.setHours(wakeH, wakeM, 0, 0);

    if (sleepDate > wakeDate) {
      sleepDate.setDate(sleepDate.getDate() - 1);
    }

    return {
      sleepStart: sleepDate.toISOString().slice(0, 19),
      sleepEnd: wakeDate.toISOString().slice(0, 19),
      qualityOfSleep: Number(formData.sleepQuality),
      stressLevel: Number(formData.stressLevel),
      physicalActivityMinutes: activityApiValues[formData.physicalActivity] || 15,
      bmiCategory: 1,
      bloodPressure: formData.bloodPressure === "" ? 120 : Number(formData.bloodPressure),
      notes: `Atividade ${formData.physicalActivity} com qualidade ${formData.sleepQuality}/10 e estresse ${formData.stressLevel}/10.`,
      heartRate: formData.heartRate === "" ? 72 : Number(formData.heartRate),
      dailySteps: formData.dailySteps === "" ? 5000 : Number(formData.dailySteps),
      mentalFatigue: Number(formData.mentalFatigue),
      screenBeforeSleep: Boolean(formData.habits.screens),
      caffeine: Boolean(formData.habits.caffeine),
      alcohol: Boolean(formData.habits.alcohol)
    };
  };

  const handleSave = async () => {
    setIsSaving(true);

    const entry = {
      id: Date.now(),
      createdAt: new Date().toISOString(),
      date: new Date().toLocaleDateString("pt-BR"),
      weekday: dateParts.weekday,
      physicalActivity: formData.physicalActivity,
      sleepTime: formData.sleepTime,
      wakeTime: formData.wakeTime,
      duration: calculatedDuration?.formatted || "Nao calculado",
      stressLevel: `${formData.stressLevel}/10`,
      quality: `${formData.sleepQuality}/10`,
      mentalFatigue: `${formData.mentalFatigue}/10`,
      heartRate: formData.heartRate ? `${formData.heartRate} bpm` : "",
      bloodPressure: formData.bloodPressure === "" ? "" : Number(formData.bloodPressure),
      steps: formData.dailySteps,
      caffeine: formData.habits.caffeine,
      phoneBeforeSleep: formData.habits.screens,
      screenTimeBeforeSleep: formData.screenTimeBeforeSleep ? `${formData.screenTimeBeforeSleep} min` : "",
      alcohol: formData.habits.alcohol,
      note: `Atividade ${formData.physicalActivity.toLowerCase()} com qualidade ${formData.sleepQuality}/10 e estresse ${formData.stressLevel}/10.`,
      sleepScore: Math.round(
        (Number(formData.sleepQuality) * 5) +
        (10 - Number(formData.stressLevel)) * 3 +
        (10 - Number(formData.mentalFatigue)) * 2
      ),
      detectedDisturbance: Number(formData.stressLevel) >= 8 ? "Estresse elevado" : "Nenhum sinal relevante",
      goal: calculatedDuration ? "Registro completo" : "Registro parcial",
      tone: Number(formData.sleepQuality) >= 8 ? "tertiary" : "secondary"
    };

    saveDiaryEntry(entry);

    try {
      await saveSleepRecord(buildApiPayload());
      setSaveMessage("Registro salvo e enviado ao backend. Seus insights podem ser atualizados com esses novos dados.");
    } catch (error) {
      console.error("Erro ao enviar registro de sono para API:", error);
      setSaveMessage("Registro salvo localmente. Quando o backend estiver disponivel, este envio podera ser integrado automaticamente.");
    } finally {
      setIsSaving(false);
    }
  };

  return (
    <div className="app-shell">
      <SideNav active="diario" />
      <main className="main main--narrow">
        <header className="page-header">
          <div>
            <span className="kicker">Registro Diario</span>
            <h2>Como voce descansou?</h2>
          </div>
          <div className="date-block">
            <p>{dateParts.weekday}</p>
            <strong>{dateParts.fullDate}</strong>
          </div>
        </header>

        <div className="diary-grid">
          <section className="card sleep-duration">
            <div className="card-title-row">
              <div>
                <h3>Duracao do Sono</h3>
                <p>Calculada automaticamente a partir dos horarios informados.</p>
              </div>
              <span className="round-icon round-icon--primary">
                <MaterialIcon>bedtime</MaterialIcon>
              </span>
            </div>
            <div className="duration-value">
              <strong>{calculatedDuration ? calculatedDuration.hours : "--"}</strong>
              <span>{calculatedDuration ? `${String(calculatedDuration.minutes).padStart(2, "0")} min` : "aguardando"}</span>
            </div>
            <div className="diary-form-grid">
              <Field label="Atividade fisica">
                <select name="physicalActivity" value={formData.physicalActivity} onChange={handleFieldChange}>
                  {activityOptions.map((option) => (
                    <option key={option} value={option}>{option}</option>
                  ))}
                </select>
              </Field>
              <Field label="Horario que dormiu">
                <input type="time" name="sleepTime" value={formData.sleepTime} onChange={handleFieldChange} />
              </Field>
              <Field label="Horario que acordou">
                <input type="time" name="wakeTime" value={formData.wakeTime} onChange={handleFieldChange} />
              </Field>
              <Field label="Duracao calculada">
                <input type="text" value={calculatedDuration?.formatted || "Preencha os horarios"} readOnly />
              </Field>
            </div>
            <div className="note-row">
              <MaterialIcon>schedule</MaterialIcon>
              <p>{calculatedDuration ? `Sono total estimado em ${calculatedDuration.label}, inclusive quando passa da meia-noite.` : "Informe os dois horarios para calcular a duracao automaticamente."}</p>
            </div>
          </section>

          <section className="card quality-card">
            <div className="inline-title">
              <MaterialIcon filled>star</MaterialIcon>
              <h3>Qualidade do Sono</h3>
            </div>
            <div className="quality-circle">
              <strong>{formData.sleepQuality}<span>/10</span></strong>
            </div>
            <p>{getSleepQualityLabel(Number(formData.sleepQuality))}</p>
            <input className="range range--secondary" type="range" min="1" max="10" name="sleepQuality" value={formData.sleepQuality} onChange={handleFieldChange} aria-label="Qualidade do sono" />
          </section>

          <section className="card stress-card">
            <h3>Nivel de Estresse</h3>
            <div className="slider-card__value">{formData.stressLevel}/10</div>
            <input className="range" type="range" min="1" max="10" name="stressLevel" value={formData.stressLevel} onChange={handleFieldChange} aria-label="Nivel de estresse" />
            <div className="note-row">
              <MaterialIcon>psychology</MaterialIcon>
              <p>O estresse em {formData.stressLevel}/10 ajuda a contextualizar sua latencia e recuperacao do sono.</p>
            </div>
          </section>

          <section className="card habits-card">
            <h3>Habitos Pre-Sono</h3>
            <div className="habit-list">
              <HabitToggle icon="coffee" label="Cafeina" name="caffeine" checked={formData.habits.caffeine} onChange={handleHabitChange} />
              <HabitToggle icon="smartphone" label="Telas" name="screens" checked={formData.habits.screens} onChange={handleHabitChange} />
              <HabitToggle icon="wine_bar" label="Alcool" name="alcohol" checked={formData.habits.alcohol} onChange={handleHabitChange} />
            </div>
          </section>

          <section className="card slider-card">
            <div className="inline-title inline-title--left">
              <MaterialIcon>neurology</MaterialIcon>
              <h3>Fadiga Mental</h3>
            </div>
            <div className="slider-card__value">{formData.mentalFatigue}/10</div>
            <input className="range range--secondary" type="range" min="1" max="10" name="mentalFatigue" value={formData.mentalFatigue} onChange={handleFieldChange} aria-label="Fadiga mental" />
            <div className="fatigue-meter" aria-hidden="true">
              <span style={{ width: `${Number(formData.mentalFatigue) * 10}%` }} />
            </div>
            <div className="note-row note-row--compact">
              <MaterialIcon>lightbulb</MaterialIcon>
              <p>Sua fadiga mental em {formData.mentalFatigue}/10 pode impactar diretamente a qualidade do sono.</p>
            </div>
          </section>

          <section className="card optional-card">
            <div className="card-title-row">
              <div>
                <span className="kicker">Dados opcionais</span>
                <h3>Contexto complementar</h3>
                <p>Esses campos podem ficar vazios sem bloquear seu registro.</p>
              </div>
              <span className="round-icon round-icon--secondary">
                <MaterialIcon>favorite</MaterialIcon>
              </span>
            </div>
            <div className="diary-form-grid">
              <Field label="Frequencia cardiaca">
                <input type="number" name="heartRate" value={formData.heartRate} onChange={handleFieldChange} min="0" placeholder="72" />
              </Field>
              <Field label="Pressao arterial">
                <input
                  type="number"
                  name="bloodPressure"
                  value={formData.bloodPressure}
                  onChange={handleFieldChange}
                  min="0"
                  placeholder="120"
                  inputMode="numeric"
                />
              </Field>
              <Field label="Passos diarios">
                <input type="number" name="dailySteps" value={formData.dailySteps} onChange={handleFieldChange} min="0" placeholder="7200" />
              </Field>
              <Field label="Tempo de tela antes de dormir">
                <input type="number" name="screenTimeBeforeSleep" value={formData.screenTimeBeforeSleep} onChange={handleFieldChange} min="0" placeholder="30" />
                <span className="field-hint">{formData.screenTimeBeforeSleep ? `${formData.screenTimeBeforeSleep} min` : "Opcional, em minutos"}</span>
              </Field>
            </div>
          </section>
        </div>

        <footer className="save-panel">
          <div>
            <span className="round-icon round-icon--primary">
              <MaterialIcon>auto_awesome</MaterialIcon>
            </span>
            <p>{saveMessage || "Ao salvar, nossa IA atualizara seus insights personalizados para a proxima noite."}</p>
          </div>
          <button className="btn btn--primary btn--large" type="button" onClick={handleSave} disabled={isSaving}>
            {isSaving ? "Salvando..." : "Salvar Registro"}
          </button>
        </footer>
      </main>
      <MobileNav active="diario" />
    </div>
  );
}

function Field({ label, children }) {
  return (
    <label className="field diary-field">
      <span>{label}</span>
      <div>{children}</div>
    </label>
  );
}

function HabitToggle({ icon, label, name, checked = false, onChange }) {
  return (
    <label className="habit-toggle">
      <MaterialIcon>{icon}</MaterialIcon>
      <span>{label}</span>
      <input type="checkbox" name={name} checked={checked} onChange={onChange} />
      <i aria-hidden="true" />
    </label>
  );
}
