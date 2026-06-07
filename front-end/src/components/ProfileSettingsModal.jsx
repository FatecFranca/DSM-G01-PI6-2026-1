import { useEffect, useMemo, useState } from "react";
import MaterialIcon from "./MaterialIcon.jsx";
import { calculateBmi, formatBmi, getBmiClassification } from "../utils/health.js";

const defaultFormData = {
  name: "",
  birthDate: "",
  sleepGoal: "",
  theme: "light",
  profession: "",
  gender: "Prefiro nao informar",
  weight: "",
  height: ""
};

export default function ProfileSettingsModal({
  isOpen,
  onClose,
  initialData = defaultFormData,
  onSave
}) {
  const [formData, setFormData] = useState({ ...defaultFormData, ...initialData });

  const bmi = useMemo(
    () => calculateBmi(formData.weight, formData.height),
    [formData.height, formData.weight]
  );
  const bmiClassification = getBmiClassification(bmi);

  useEffect(() => {
    if (isOpen) {
      setFormData({ ...defaultFormData, ...initialData });
      document.body.classList.add("modal-open");
    }

    return () => document.body.classList.remove("modal-open");
  }, [initialData, isOpen]);

  useEffect(() => {
    if (!isOpen) return undefined;

    const handleKeyDown = (event) => {
      if (event.key === "Escape") {
        onClose();
      }
    };

    window.addEventListener("keydown", handleKeyDown);
    return () => window.removeEventListener("keydown", handleKeyDown);
  }, [isOpen, onClose]);

  if (!isOpen) return null;

  const handleChange = (event) => {
    const { name, value } = event.target;
    setFormData((currentData) => ({ ...currentData, [name]: value }));
  };

  const handleSubmit = (event) => {
    event.preventDefault();
    onSave?.(formData);
    onClose();
  };

  const handleBackdropClick = (event) => {
    if (event.target === event.currentTarget) {
      onClose();
    }
  };

  return (
    <div className="profile-settings-modal" role="presentation" onMouseDown={handleBackdropClick}>
      <section
        className="profile-settings-modal__panel"
        role="dialog"
        aria-modal="true"
        aria-labelledby="profile-settings-title"
      >
        <header className="profile-settings-modal__header">
          <div>
            <span className="kicker">Perfil</span>
            <h2 id="profile-settings-title">Configuracoes</h2>
          </div>
          <button className="icon-button" type="button" aria-label="Fechar configuracoes" onClick={onClose}>
            <MaterialIcon>close</MaterialIcon>
          </button>
        </header>

        <form className="profile-settings-form" onSubmit={handleSubmit}>
          <Field label="Nome">
            <input
              type="text"
              name="name"
              value={formData.name}
              onChange={handleChange}
              placeholder="Seu nome"
            />
          </Field>

          <Field label="Data de nascimento">
            <input
              type="text"
              name="birthDate"
              value={formData.birthDate}
              onChange={handleChange}
              inputMode="numeric"
              pattern="[0-9]{2}/[0-9]{2}/[0-9]{4}"
              placeholder="DD/MM/AAAA"
            />
          </Field>

          <Field label="Meta de sono">
            <input
              type="text"
              name="sleepGoal"
              value={formData.sleepGoal}
              onChange={handleChange}
              placeholder="Ex.: 8h 00m"
            />
          </Field>

          <Field label="Tema do app">
            <select name="theme" value={formData.theme} onChange={handleChange}>
              <option value="light">Claro</option>
              <option value="dark">Escuro</option>
            </select>
          </Field>

          <Field label="Genero">
            <select name="gender" value={formData.gender} onChange={handleChange}>
              <option value="Feminino">Feminino</option>
              <option value="Masculino">Masculino</option>
            </select>
          </Field>

          <Field label="Peso (kg)">
            <input
              type="number"
              name="weight"
              value={formData.weight}
              onChange={handleChange}
              min="0"
              step="0.1"
              placeholder="70"
            />
          </Field>

          <Field label="Altura (cm)">
            <input
              type="number"
              name="height"
              value={formData.height}
              onChange={handleChange}
              min="0"
              step="0.1"
              placeholder="170"
            />
          </Field>

          <Field label="Cargo profissional" wide>
            <input
              type="text"
              name="profession"
              value={formData.profession}
              onChange={handleChange}
              placeholder="Ex.: Designer de Produto"
            />
          </Field>

          <div className="form-helper-card profile-settings-form__field profile-settings-form__field--wide">
            <span className="kicker">IMC automatico</span>
            <strong>{formatBmi(bmi)}</strong>
            <p>{bmiClassification || "Preencha peso e altura para gerar seu IMC automaticamente."}</p>
          </div>

          <footer className="profile-settings-modal__actions">
            <button className="btn btn--soft" type="button" onClick={onClose}>
              Cancelar
            </button>
            <button className="btn btn--primary" type="submit">
              Salvar
            </button>
          </footer>
        </form>
      </section>
    </div>
  );
}

function Field({ label, wide = false, children }) {
  const className = wide
    ? "field profile-settings-form__field profile-settings-form__field--wide"
    : "field profile-settings-form__field";

  return (
    <label className={className}>
      <span>{label}</span>
      <div>{children}</div>
    </label>
  );
}
