import MaterialIcon from "../components/MaterialIcon.jsx";
import TopBar from "../components/TopBar.jsx";

const heroImage =
  "https://lh3.googleusercontent.com/aida-public/AB6AXuBROHucUrC1aDRPiOhC0pVG_JBiT14EDyFBfl2THXxMnPfUZL9fqsO0DnVZh88VwNSjVGEYiHsm3uJfl25WYMwzCUULLGmq4p3auxL_6EQ1KivQJtOq2GYW2YU3l87x0AVWIZTz_Jcp6t-S_hzLjHkjtKae0GKxRIzW-jgKVBL6_h9qmNJd_yFqGsNe0LyzJhIG3tUkwqJUSF1-KyhX5s_dFodKXeyNO-KKWm82bhIK3szgnIBahz6rsw4AGH2KqR0YrCaO8QU97Wk";
const particlesImage =
  "https://lh3.googleusercontent.com/aida-public/AB6AXuBtw6VHXUm_K5wO2HWm74Bd96H6iUdUpOH8Cz-H-1pph5kzxt16Tyakssm2U5wZjDFuuP-icMPjwWcY__hMNl4gWq1kziIjeCaGOcUoP82hSqwKbGSF1_19x270RhDQKt1JDcClXY1Pvs6A0panwQsrTZzB1em_nwfNZiHZwp6duPU2gbgzxjtqW4-PNW08RqiZTvm49hBGPzzUbPR85SM0sGRWVkmSTdxyJr48iYYApIQs8LPc2agPVwT6YlFIU03tvaLdPJx8dNA";

export default function OnboardingPage() {
  return (
    <div className="landing">
      <TopBar landing />
      <main>
        <section className="hero-section">
          <div className="hero-section__content">
            <span className="badge">Inteligência Artificial & Bem-estar</span>
            <h1>Seu descanso perfeito, guiado por IA.</h1>
            <p>Entenda seus padrões de sono e melhore seu descanso com insights personalizados gerados pela nossa tecnologia exclusiva.</p>
            <div className="hero-actions">
              <a className="btn btn--primary btn--large" href="#/login">Começar</a>
              <a className="btn btn--white btn--large" href="#features">Saiba Mais</a>
            </div>
          </div>
          <div className="hero-media">
            <img src={heroImage} alt="Quarto moderno e tranquilo com luz suave" />
            <div className="floating-stat">
              <div>
                <span className="round-icon round-icon--tertiary"><MaterialIcon>check_circle</MaterialIcon></span>
                <div>
                  <strong>Qualidade do Sono</strong>
                  <p>Melhora de 24% esta semana</p>
                </div>
              </div>
              <div className="progress"><span style={{ width: "88%" }} /></div>
            </div>
          </div>
        </section>

        <section className="features-section" id="features">
          <div className="center-heading">
            <h2>Tecnologia que cuida de você</h2>
            <p>Nossa IA analisa mais de 50 variáveis para criar o ambiente e a rotina ideais para o seu biótipo.</p>
          </div>
          <div className="feature-grid">
            <article className="feature-card feature-card--large">
              <div>
                <MaterialIcon className="feature-icon">neurology</MaterialIcon>
                <h3>Análise Preditiva de Ciclos</h3>
                <p>Identificamos o momento exato para o seu despertar, garantindo que você acorde na fase mais leve do sono.</p>
                <button type="button">Ver detalhes <MaterialIcon>arrow_forward</MaterialIcon></button>
              </div>
              <img src={particlesImage} alt="Partículas abstratas representando inteligência artificial" />
            </article>
            <FeatureCard icon="eco" title="Ajuste Ambiental" text="Sugestões em tempo real para temperatura e iluminação baseadas na sua respiração." tone="green" />
            <FeatureCard icon="insights" title="Relatórios Diários" text="Receba um resumo editorial das suas noites com dicas práticas de higiene do sono." tone="blue" />
            <article className="feature-card feature-card--wide">
              <div>
                <MaterialIcon className="feature-icon">bedtime</MaterialIcon>
                <h3>Santuário de Sons</h3>
                <p>Uma biblioteca infinita de frequências binaurais e sons da natureza gerados por IA para sua mente relaxar.</p>
              </div>
              <div className="sound-widget">
                <div><strong>FLUXO ATUAL</strong><span>432Hz</span></div>
                <div className="sound-bars">{[40, 60, 90, 70, 100, 80, 50].map((height) => <i style={{ height: `${height}%` }} key={height} />)}</div>
              </div>
            </article>
          </div>
        </section>

        <section className="cta-section">
          <h2>Pronto para transformar suas noites?</h2>
          <p>Junte-se a milhares de pessoas que já redescobriram o prazer de acordar descansado.</p>
          <a className="btn btn--light btn--large" href="#/cadastro">Criar minha conta gratuita</a>
        </section>
      </main>
      <footer className="landing-footer">
        <strong>Santuário do Sono</strong>
        <nav>
          <a href="#">Privacidade</a>
          <a href="#">Termos de Uso</a>
          <a href="#">Cookies</a>
          <a href="#">Contato</a>
        </nav>
        <span>© 2024 Santuário do Sono. Todos os direitos reservados.</span>
      </footer>
    </div>
  );
}

function FeatureCard({ icon, title, text, tone }) {
  return (
    <article className={`feature-card feature-card--${tone}`}>
      <MaterialIcon className="feature-icon">{icon}</MaterialIcon>
      <h3>{title}</h3>
      <p>{text}</p>
    </article>
  );
}
