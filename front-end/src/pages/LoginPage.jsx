import { useState } from "react";
import MaterialIcon from "../components/MaterialIcon.jsx";
import { loginUser } from "../utils/auth.js";

const heroImage =
  "https://lh3.googleusercontent.com/aida-public/AB6AXuDg92xm7OB3TwtHW08QAPl5RTin6-35Ijlfm2l8GvvsS9kmT3KlCYh7r3oMFabsdW6VukinrLxkgD7-lAa2uBpOPUE7r9-pwmDLY6QdipYZfngyTvDt5dkG4RolSLQy9Dy_wD6_jw6-J-0mrd4iQZuYKEt9OyB7uao8yRs3-UiS0uXdZgc3apHvyoewDlErbkhsGtqtwc0DkFnLJndgIxSdH4VKGwY-4mzxDRdqpqplS2-FIjmcDOxSkAiM-2ZFYF_AEym53sBkPko";
const googleLogo =
  "https://lh3.googleusercontent.com/aida-public/AB6AXuCDcvxgE0sICXzMM8iewdP0eIEwe8na2MI4-ONBzedefRKpDOK4AW8q5KA3qS2S6Z7y7ouq7DZptanfRKV2m2BZGTsGehdcBoJaTczHsuzSXv2GOJ4EeDYSWkYvF0u5C96PQrIMww6Ae1OpKlgfZLWHL2jTAvYgIX8MXoffWth6Dpd6GVWQT4dkVqsYc95kqHrJVOscgKDjB2WtOdu3wrCd2RolZ369tkTlHaJn4RxX_qD16hcq-79LO4apA2uoh4e111FRoh1wlN4";

const initialLoginData = {
  email: "",
  password: ""
};

export default function LoginPage() {
  const [formData, setFormData] = useState(initialLoginData);
  const [errorMessage, setErrorMessage] = useState("");

  const handleChange = (event) => {
    const { name, value } = event.target;
    setFormData((currentData) => ({ ...currentData, [name]: value }));
  };

  const handleSubmit = async (event) => {
    event.preventDefault();

    const result = await loginUser(formData.email, formData.password);

    if (!result.success) {
      setErrorMessage(result.message);
      return;
    }

    setErrorMessage("");
    window.location.hash = "/home";
  };

  return (
    <main className="login-page">
      <section className="login-visual" aria-label="Apresentacao do Santuario do Sono">
        <div className="login-visual__brand">Santuario do Sono</div>
        <div className="login-visual__copy">
          <h1>Bem-vindo de volta ao seu descanso.</h1>
          <p>Reconecte-se com seu ritmo natural. Sua jornada para uma noite tranquila comeca aqui.</p>
        </div>
        <div className="login-visual__steps" aria-hidden="true">
          <span className="active" />
          <span />
          <span />
        </div>
        <div className="login-glow login-glow--top" />
        <div className="login-glow login-glow--bottom" />
        <img className="login-visual__image" src={heroImage} alt="" />
      </section>

      <section className="login-panel" aria-label="Formulario de login">
        <div className="login-panel__inner">
          <section className="login-card">
            <header className="form-heading">
              <h2>Entrar</h2>
              <p>Acesse sua conta para continuar sua jornada de sono.</p>
            </header>

            <form onSubmit={handleSubmit}>
              <Field id="email" label="E-mail" placeholder="nome@exemplo.com" type="email" value={formData.email} onChange={handleChange} />
              <Field
                id="password"
                label="Senha"
                placeholder="........"
                type="password"
                action={<a href="#">Esqueceu sua senha?</a>}
                icon="visibility"
                value={formData.password}
                onChange={handleChange}
              />

              {errorMessage ? <p className="form-feedback form-feedback--error">{errorMessage}</p> : null}

              <button className="btn btn--primary btn--full btn--large" type="submit">
                Entrar
              </button>
            </form>

            <div className="divider"><span>Ou entre com</span></div>

            <div className="social-buttons">
              <button type="button"><img src={googleLogo} alt="" />Google</button>
              <button type="button"><MaterialIcon filled>ios</MaterialIcon>Apple</button>
            </div>

            <p className="login-link">Ainda nao tem uma conta? <a href="#/cadastro">Criar conta</a></p>
          </section>

          <p className="login-page__footer">© 2024 Santuario do Sono. Minimalismo Etereo.</p>
        </div>
      </section>
    </main>
  );
}

function Field({ id, label, icon, type = "text", placeholder, action, value, onChange }) {
  return (
    <label className="field" htmlFor={id}>
      <span className="field__row">
        <span>{label}</span>
        {action}
      </span>
      <div>
        <input id={id} name={id} type={type} placeholder={placeholder} value={value} onChange={onChange} />
        {icon && (
          <button type="button" aria-label="Mostrar senha">
            <MaterialIcon>{icon}</MaterialIcon>
          </button>
        )}
      </div>
    </label>
  );
}
