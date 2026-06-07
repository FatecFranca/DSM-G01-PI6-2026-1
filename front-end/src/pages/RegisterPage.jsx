import { useState } from "react";
import MaterialIcon from "../components/MaterialIcon.jsx";
import { registerUser } from "../utils/auth.js";

const googleLogo =
  "https://lh3.googleusercontent.com/aida-public/AB6AXuDpSb9tu2kvkG4l985pI2SmI5r450-x0ED2Lwgslbp55W0m0yC6Njddn7zfdvFHtf5ELHCGY6lw-UUWKHLH_7Tc7owuOH2eiSd_rUPpELDJJQps_I3CGJsJkRULit4LdxyVjAfo1ZQFACdDQCIGg-K4S2UHVpMCeh61P1MIL6aVhSnM3pFA7mOQo-pK0lwWuSdziu-RsO07nnv1LyDlen0BnmenHLnml2mtO08GIvwIStK00bQioAT9S3x_0BbhmYOShwMInR85QF0";

const initialFormData = {
  name: "",
  email: "",
  password: "",
  birthDate: "",
  gender: "",
  profession: "",
  sleepGoal: "",
  weight: "",
  height: ""
};

export default function RegisterPage() {
  const [formData, setFormData] = useState(initialFormData);
  const [feedback, setFeedback] = useState({ type: "", message: "" });

  const handleChange = (event) => {
    const { name, value } = event.target;
    setFormData((currentData) => ({ ...currentData, [name]: value }));
  };

  const handleSubmit = async (event) => {
    event.preventDefault();

    const result = await registerUser({
      ...formData,
      birthDate: formatBirthDate(formData.birthDate)
    });

    if (!result.success) {
      setFeedback({ type: "error", message: result.message });
      return;
    }

    setFeedback({
      type: "success",
      message: "Cadastro concluido. Agora faca login para acessar seu painel."
    });

    window.setTimeout(() => {
      window.location.hash = "/login";
    }, 700);
  };

  return (
    <main className="register-page">
      <div className="glow glow--primary" />
      <div className="glow glow--secondary" />

      <section className="register-intro">
        <div>
          <h2>Santuario do Sono</h2>
          <span />
        </div>
        <h1>Sua jornada para o <em>descanso profundo</em> comeca aqui.</h1>
        <p>Crie sua conta para acompanhar seus padroes de sono, registrar noites e receber insights personalizados.</p>
        <article className="insight-card">
          <div>
            <span className="round-icon round-icon--tertiary"><MaterialIcon>auto_awesome</MaterialIcon></span>
            <div>
              <strong>Fluxo provisório</strong>
              <p>Cadastro local para testes</p>
            </div>
          </div>
          <blockquote>
            "Esta autenticacao usa localStorage apenas para validacao do front-end e sera substituida por API no backend."
          </blockquote>
        </article>
      </section>

      <section className="register-card">
        <div className="form-heading">
          <h2>Criar conta</h2>
          <p>Preencha os dados obrigatorios para continuar.</p>
        </div>

        <form onSubmit={handleSubmit}>
          <Field id="name" name="name" label="Nome completo" placeholder="Como deseja ser chamado?" value={formData.name} onChange={handleChange} />
          <Field id="email" name="email" label="E-mail" placeholder="seu@email.com" type="email" value={formData.email} onChange={handleChange} />
          <Field id="password" name="password" label="Senha" placeholder="........" type="password" icon="visibility" value={formData.password} onChange={handleChange} />
          <Field id="birthDate" name="birthDate" label="Data de nascimento" placeholder="DDMMAAAA ou DD/MM/AAAA" value={formData.birthDate} onChange={handleChange} />
          <SelectField id="gender" name="gender" label="Genero" value={formData.gender} onChange={handleChange}>
            <option value="" disabled>Selecione seu genero</option>
            <option value="F">Feminino</option>
            <option value="M">Masculino</option>
          </SelectField>
          <Field id="profession" name="profession" label="Cargo profissional" placeholder="Ex.: Analista de Produto" value={formData.profession} onChange={handleChange} />
          <Field id="sleepGoal" name="sleepGoal" label="Meta de sono" placeholder="Ex.: 8h 00m" value={formData.sleepGoal} onChange={handleChange} />
          <Field id="weight" name="weight" label="Peso (opcional, kg)" placeholder="70" type="number" min="0" step="0.1" value={formData.weight} onChange={handleChange} />
          <Field id="height" name="height" label="Altura (opcional, cm)" placeholder="170" type="number" min="0" step="0.1" value={formData.height} onChange={handleChange} />

          <div className="form-helper-card">
            <span className="kicker">Observacao</span>
            <strong>Teste local</strong>
            <p>As credenciais ficam apenas no navegador durante a fase de prototipacao e devem ser migradas depois para o backend.</p>
          </div>

          {feedback.message ? (
            <p className={`form-feedback ${feedback.type === "error" ? "form-feedback--error" : ""}`}>
              {feedback.message}
            </p>
          ) : null}

          <button className="btn btn--primary btn--full btn--large" type="submit">
            Criar conta
            <MaterialIcon>arrow_forward</MaterialIcon>
          </button>
        </form>

        <div className="divider"><span>Ou cadastre-se com</span></div>
        <div className="social-buttons">
          <button type="button"><img src={googleLogo} alt="" />Google</button>
          <button type="button"><MaterialIcon filled>ios</MaterialIcon>Apple</button>
        </div>
        <p className="login-link">Ja tem uma conta? <a href="#/login">Fazer login</a></p>
      </section>
    </main>
  );
}

function Field({ id, name, label, icon, type = "text", placeholder, value, onChange, min, step }) {
  return (
    <label className="field" htmlFor={id}>
      <span>{label}</span>
      <div>
        <input id={id} name={name} type={type} placeholder={placeholder} value={value} onChange={onChange} min={min} step={step} />
        {icon && (
          <button type="button" aria-label="Mostrar senha">
            <MaterialIcon>{icon}</MaterialIcon>
          </button>
        )}
      </div>
    </label>
  );
}

function SelectField({ id, name, label, value, onChange, children }) {
  return (
    <label className="field" htmlFor={id}>
      <span>{label}</span>
      <div>
        <select id={id} name={name} value={value} onChange={onChange}>
          {children}
        </select>
      </div>
    </label>
  );
}

function formatBirthDate(value) {
  const digits = String(value || "").replace(/\D/g, "");

  if (digits.length !== 8) {
    return String(value || "").trim();
  }

  return `${digits.slice(0, 2)}/${digits.slice(2, 4)}/${digits.slice(4)}`;
}
