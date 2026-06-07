# Servico Python de IA com RabbitMQ

Este servico consome mensagens da mesma exchange do backend .NET e publica a resposta com os resultados de **2 modelos separados** no mesmo payload.

## Fluxo

- Consome da exchange `sleep.exchange` com routing key `ai.processing`.
- Executa:
  - modelo de classificacao (`problem`)
  - modelo de regressao (`score`)
- Publica em `sleep.exchange` com routing key `sleep.return`.

O backend .NET consome `sleep.return` e usa `sleepRecordId`, `problem` e `score`.

## Instalacao

```bash
pip install -r requirements-rabbitmq-service.txt
```

## Execucao

```bash
python rabbitmq_ai_service.py
```

## Variaveis de ambiente (opcionais)

- `RABBITMQ_HOST` (default: `localhost`)
- `RABBITMQ_PORT` (default: `5672`)
- `RABBITMQ_USERNAME` (default: `guest`)
- `RABBITMQ_PASSWORD` (default: `guest`)
- `RABBITMQ_VHOST` (default: `/`)
- `RABBITMQ_EXCHANGE` (default: `sleep.exchange`)
- `RABBITMQ_REQUEST_ROUTING_KEY` (default: `ai.processing`)
- `RABBITMQ_RESPONSE_ROUTING_KEY` (default: `sleep.return`)
- `RABBITMQ_CONSUME_QUEUE` (default: `ai.processing.queue`)
- `CLASSIFIER_MODEL_PATH` (default: `service/models/rf_combined_sleep_model.pkl`)
- `REGRESSOR_MODEL_PATH` (default: `service/models/sleep_score_model.pkl`)

## Estrutura de payload de resposta

Exemplo enviado para `sleep.return`:

```json
{
  "sleepRecordId": 0,
  "problem": "No Disorder",
  "score": 7.42,
  "modelResults": {
    "classification": {
      "prediction": "No Disorder",
      "confidence": 0.93,
      "source": "model"
    },
    "regression": {
      "prediction": 7.42,
      "source": "model"
    }
  }
}
```

> Observacao: se os modelos `.pkl` nao estiverem presentes, o servico usa fallback heuristico para manter o fluxo funcionando.
