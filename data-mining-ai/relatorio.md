# 🧠 ANÁLISE DE QUALIDADE DO SONO UTILIZANDO MACHINE LEARNING

## 1. Introdução

A qualidade do sono é um fator essencial para a saúde física e mental. Diversos aspectos do estilo de vida moderno, como estresse elevado, uso de dispositivos eletrônicos e hábitos irregulares, impactam diretamente o padrão de sono dos indivíduos.


Este trabalho tem como objetivo desenvolver um sistema capaz de:
- Identificar possíveis distúrbios do sono
- Estimar a qualidade do sono de um indivíduo
- Analisar o impacto de hábitos comportamentais

---

## 2. Objetivos

### 2.1 Objetivo Geral

Desenvolver modelos de Machine Learning para análise de sono utilizando dados fisiológicos e comportamentais.

### 2.2 Objetivos Específicos

- Realizar análise exploratória dos dados
- Aplicar técnicas de pré-processamento
- Treinar modelos de classificação e regressão
- Avaliar desempenho dos modelos
- Integrar diferentes fontes de dados

---

## 3. Base de Dados

Foram utilizados dois conjuntos de dados distintos.

### 3.1 Dataset de Saúde e Sono

Contém informações fisiológicas e médicas, incluindo:
- Idade
- Duração do sono
- Qualidade do sono
- Nível de estresse
- IMC
- Pressão arterial
- Frequência cardíaca
- Passos diários
- Distúrbio do sono (variável alvo)

### 3.2 Dataset Comportamental

Contém dados relacionados ao estilo de vida:
- Uso de celular antes de dormir
- Consumo de cafeína
- Estresse
- Fadiga mental
- Score de qualidade do sono

---

## 4. Metodologia

### 4.1 Abordagem Geral

O problema foi dividido em duas tarefas distintas:

- Classificação → identificar distúrbios do sono
- Regressão → prever o score de qualidade do sono

Essa divisão permitiu utilizar diferentes estratégias para cada tipo de problema.

---

## 5. Pré-processamento dos Dados

### 5.1 Padronização

Os nomes das colunas foram normalizados para facilitar o processamento:

```python
df.columns = df.columns.str.lower().str.replace(" ", "_")
```

## 5.2 Tratamento de Valores Ausentes

- Variáveis numéricas: preenchidas com a **mediana**
- Variáveis categóricas: preenchidas com a **moda**

---

## 5.3 Transformação de Variáveis

### Pressão Arterial

Os valores originalmente no formato `"120/80"` foram convertidos para uma única variável utilizando a fórmula da pressão arterial média:

MAP = (Sistólica + 2 × Diastólica) / 3


---

## 5.4 Codificação de Variáveis Categóricas

Foi aplicado **One-Hot Encoding** para:

- Gênero
- Ocupação
- Categoria de IMC

---

# 6. Modelagem

## 6.1 Classificação

Para a tarefa de classificação, foi utilizada uma abordagem de **combinação de datasets**.

### Estratégia

- Uso do dataset principal (dados fisiológicos)
- Adição do dataset comportamental
- Geração de rótulos artificiais (**pseudo-labeling**)

### Modelos utilizados

- Random Forest
- XGBoost

---

## 6.2 Regressão

Para regressão, foi utilizado apenas o dataset comportamental.

### Motivo

Evitar viés causado por dados artificiais.

### Modelo utilizado

- Random Forest Regressor

---

# 7. Resultados

## 7.1 Classificação

| Modelo         | Accuracy | Cross-validation |
|----------------|----------|------------------|
| Random Forest  | 95%      | 86.9%            |
| XGBoost        | 97%      | 83%              |

O modelo **Random Forest** apresentou maior estabilidade.

---

## 7.2 Regressão

- **R² ≈ 0.79**
- **MAE ≈ 0.62**
- **RMSE ≈ 0.78**

O modelo apresentou boa capacidade de previsão.

---

# 8. Análise dos Resultados

As variáveis mais relevantes identificadas foram:

- Qualidade do sono
- Pressão arterial média
- Nível de estresse
- Fadiga mental
- Idade

Isso indica forte relação entre fatores fisiológicos e comportamentais.

---

# 9. Discussão

A utilização de datasets combinados melhorou significativamente a performance da classificação, porém introduziu dados sintéticos, o que pode gerar viés.

Já a regressão apresentou maior estabilidade ao utilizar apenas dados reais.

A escolha de separar os problemas em duas abordagens distintas mostrou-se eficiente.

---

# 10. Limitações

- Uso de dados sintéticos na classificação
- Base de dados limitada
- Dependência da qualidade dos dados inseridos pelo usuário

---

# 11. Trabalhos Futuros

- Integração com backend (API)
- Implementação de sistema de recomendação
- Coleta de dados reais via aplicação
- Teste com outros algoritmos

---

# 12. Conclusão

O projeto demonstrou que técnicas de Machine Learning podem ser aplicadas com sucesso na análise de padrões de sono.
A combinação de datasets foi utilizada para aumentar a capacidade de generalização do modelo de classificação, compensando o tamanho reduzido do dataset principal. Para evitar viés, a regressão foi treinada separadamente utilizando apenas dados consistentes. Efetuando a combinação de dados fisiológicos e comportamentais permitiu criar um sistema capaz de fornecer informações relevantes ao usuário, contribuindo para a melhoria da qualidade de vida.

---

# 13. Tecnologias Utilizadas

- Python
- Pandas
- Scikit-learn
- XGBoost
- Jupyter Notebook

---

# 14. Datasets utilizados

- [Dataset fisiológico](https://www.kaggle.com/datasets/uom190346a/sleep-health-and-lifestyle-dataset/data) 
- [Dataset comportamental](https://www.kaggle.com/datasets/jayjoshi37/sleep-screen-time-and-stress-analysis)

---

# 15. Scripts pre_processamento e treinamento 

- [Script antigo do modelo de classificação](https://github.com/viivi02/Projeto-interdisciplinar-6Sem/blob/8f9251de4bf8765cf83c4f5991ba9dbf5b1ff759/data-mining-ai/pre_processamento.py)
- [Script do modelo de classificação com datasets combinados](https://github.com/viivi02/Projeto-interdisciplinar-6Sem/blob/8f9251de4bf8765cf83c4f5991ba9dbf5b1ff759/data-mining-ai/classificacao_datasets_combinados.py)
- [Script do modelo de regressão](https://github.com/viivi02/Projeto-interdisciplinar-6Sem/blob/8f9251de4bf8765cf83c4f5991ba9dbf5b1ff759/data-mining-ai/regressao_score_sono.py)
