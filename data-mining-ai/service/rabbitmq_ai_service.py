import json
import logging
import os
from dataclasses import dataclass
from typing import Any, Dict, Optional

import joblib
import pandas as pd
import pika

BASE_DIR = os.path.dirname(os.path.abspath(__file__))
DEFAULT_CLASSIFIER_PATH = os.path.join(BASE_DIR, "models", "rf_combined_sleep_model.pkl")
DEFAULT_REGRESSOR_PATH = os.path.join(BASE_DIR, "models", "sleep_score_model.pkl")


logging.basicConfig(
    level=os.getenv("LOG_LEVEL", "INFO"),
    format="%(asctime)s | %(levelname)s | %(message)s",
)
LOGGER = logging.getLogger("rabbitmq-ai-service")


@dataclass
class RabbitConfig:
    host: str = os.getenv("RABBITMQ_HOST", "localhost")
    username: str = os.getenv("RABBITMQ_USERNAME", "guest")
    password: str = os.getenv("RABBITMQ_PASSWORD", "guest")
    virtual_host: str = os.getenv("RABBITMQ_VHOST", "/")
    port: int = int(os.getenv("RABBITMQ_PORT", "5672"))
    exchange: str = os.getenv("RABBITMQ_EXCHANGE", "sleep.exchange")
    request_routing_key: str = os.getenv("RABBITMQ_REQUEST_ROUTING_KEY", "ai.processing")
    response_routing_key: str = os.getenv("RABBITMQ_RESPONSE_ROUTING_KEY", "sleep.return")
    consume_queue: str = os.getenv("RABBITMQ_CONSUME_QUEUE", "ai.processing.queue")


class TwoModelAnalyzer:
    def __init__(self, classifier_path: str, regressor_path: str) -> None:
        self.classifier = self._load_model(classifier_path, "classificador")
        self.regressor = self._load_model(regressor_path, "regressor")

    @staticmethod
    def _load_model(path: str, model_name: str) -> Optional[Any]:
        if not os.path.exists(path):
            LOGGER.warning("Modelo %s nao encontrado em: %s. Usando fallback heuristico.", model_name, path)
            return None
        model = joblib.load(path)
        LOGGER.info("Modelo %s carregado: %s", model_name, path)
        return model

    @staticmethod
    def _to_float(value: Any, default: float = 0.0) -> float:
        try:
            return float(value)
        except (TypeError, ValueError):
            return default

    @staticmethod
    def _to_int(value: Any, default: int = 0) -> int:
        try:
            return int(value)
        except (TypeError, ValueError):
            return default

    @staticmethod
    def _normalize_payload(payload: Dict[str, Any]) -> Dict[str, Any]:
        normalized: Dict[str, Any] = {}
        for key, value in payload.items():
            if key is None:
                continue
            clean_key = str(key).replace("_", "").strip().lower()
            normalized[clean_key] = value
        return normalized

    @staticmethod
    def _get_value(normalized_payload: Dict[str, Any], *aliases: str) -> Any:
        for alias in aliases:
            lookup_key = alias.replace("_", "").strip().lower()
            if lookup_key in normalized_payload:
                return normalized_payload[lookup_key]
        return None

    def _base_features(self, payload: Dict[str, Any]) -> Dict[str, Any]:
        normalized = self._normalize_payload(payload)
        return {
            "age": self._to_int(self._get_value(normalized, "age")),
            "sleep_duration_hours": self._to_float(self._get_value(normalized, "sleepDurationHours", "sleep_duration_hours")),
            "quality_of_sleep": self._to_int(self._get_value(normalized, "qualityOfSleep", "quality_of_sleep")),
            "stress_level": self._to_int(self._get_value(normalized, "stressLevel", "stress_level")),
            "mental_fatigue_score": self._to_int(self._get_value(normalized, "mentalFatigueScore", "mental_fatigue_score")),
            "physical_activity_minutes": self._to_int(self._get_value(normalized, "physicalActivityMinutes", "physical_activity_minutes")),
            "bp_mean": self._to_float(self._get_value(normalized, "bpMean", "bp_mean")),
            "heart_rate": self._to_int(self._get_value(normalized, "heartRate", "heart_rate")),
            "daily_steps": self._to_int(self._get_value(normalized, "dailySteps", "daily_steps")),
            "used_phone_before_sleep": self._to_int(self._get_value(normalized, "usedPhoneBeforeSleep", "used_phone_before_sleep")),
            "consumed_caffeine": self._to_int(self._get_value(normalized, "consumedCaffeine", "consumed_caffeine")),
            "consumed_alcohol": self._to_int(self._get_value(normalized, "consumedAlcohol", "consumed_alcohol")),
            "gender": str(self._get_value(normalized, "gender") or ""),
            "occupation": str(self._get_value(normalized, "occupation") or ""),
            "bmi_category": str(self._get_value(normalized, "bmiCategory", "bmi_category") or ""),
        }

    @staticmethod
    def _build_model_input(features: Dict[str, Any], model: Optional[Any]) -> pd.DataFrame:
        if model is None:
            return pd.DataFrame([features])

        feature_names = getattr(model, "feature_names_in_", None)
        if feature_names is None:
            return pd.DataFrame([features])

        data = pd.get_dummies(pd.DataFrame([features]), columns=["gender", "occupation", "bmi_category"], drop_first=True)
        for col in feature_names:
            if col not in data.columns:
                data[col] = 0
        data = data[list(feature_names)]
        return data

    def _fallback_classification(self, features: Dict[str, Any]) -> Dict[str, Any]:
        quality = self._to_int(features.get("quality_of_sleep"))
        stress = self._to_int(features.get("stress_level"))
        duration = self._to_float(features.get("sleep_duration_hours"))
        fatigue = self._to_int(features.get("mental_fatigue_score"))

        if quality <= 5 and (stress >= 7 or fatigue >= 7) and duration < 6.5:
            return {"prediction": "Insomnia", "confidence": 0.75, "source": "heuristic"}
        return {"prediction": "No Disorder", "confidence": 0.75, "source": "heuristic"}

    def _fallback_regression(self, features: Dict[str, Any]) -> Dict[str, Any]:
        quality = self._to_float(features.get("quality_of_sleep"))
        stress = self._to_float(features.get("stress_level"))
        duration = self._to_float(features.get("sleep_duration_hours"))
        score = max(0.0, min(10.0, (quality * 0.55) + (duration * 0.8) - (stress * 0.25)))
        return {"prediction": round(score, 2), "source": "heuristic"}

    def predict(self, payload: Dict[str, Any]) -> Dict[str, Any]:
        normalized = self._normalize_payload(payload)
        features = self._base_features(payload)

        if self.classifier is not None:
            class_input = self._build_model_input(features, self.classifier)
            class_pred = self.classifier.predict(class_input)[0]
            confidence = None
            if hasattr(self.classifier, "predict_proba"):
                probs = self.classifier.predict_proba(class_input)[0]
                confidence = float(max(probs))
            classification = {
                "prediction": str(class_pred),
                "confidence": round(confidence, 4) if confidence is not None else None,
                "source": "model",
            }
        else:
            classification = self._fallback_classification(features)

        if self.regressor is not None:
            reg_input = self._build_model_input(features, self.regressor)
            reg_pred = float(self.regressor.predict(reg_input)[0])
            regression = {"prediction": round(reg_pred, 2), "source": "model"}
        else:
            regression = self._fallback_regression(features)

        sleep_record_id = self._get_value(normalized, "sleepRecordId", "sleep_record_id")
        if sleep_record_id is None:
            sleep_record_id = 0
        try:
            sleep_record_id = int(sleep_record_id)
        except (TypeError, ValueError):
            sleep_record_id = 0

        response = {
            "sleepRecordId": sleep_record_id,
            "problem": classification["prediction"],
            "score": regression["prediction"],
            "modelResults": {
                "classification": classification,
                "regression": regression,
            },
        }
        return response


class RabbitMqAiService:
    def __init__(self, config: RabbitConfig, analyzer: TwoModelAnalyzer) -> None:
        self.config = config
        self.analyzer = analyzer
        self.connection: Optional[pika.BlockingConnection] = None
        self.channel = None

    def connect(self) -> None:
        credentials = pika.PlainCredentials(self.config.username, self.config.password)
        params = pika.ConnectionParameters(
            host=self.config.host,
            port=self.config.port,
            virtual_host=self.config.virtual_host,
            credentials=credentials,
            heartbeat=60,
        )

        self.connection = pika.BlockingConnection(params)
        self.channel = self.connection.channel()
        self.channel.exchange_declare(
            exchange=self.config.exchange,
            exchange_type="topic",
            durable=True,
        )
        self.channel.queue_declare(queue=self.config.consume_queue, durable=True)
        self.channel.queue_bind(
            queue=self.config.consume_queue,
            exchange=self.config.exchange,
            routing_key=self.config.request_routing_key,
        )
        self.channel.basic_qos(prefetch_count=1)

        LOGGER.info(
            "Conectado ao RabbitMQ em %s:%s | exchange=%s | consume_queue=%s | request_key=%s | response_key=%s",
            self.config.host,
            self.config.port,
            self.config.exchange,
            self.config.consume_queue,
            self.config.request_routing_key,
            self.config.response_routing_key,
        )

    def publish_response(self, response: Dict[str, Any]) -> None:
        if self.channel is None:
            raise RuntimeError("Canal RabbitMQ nao inicializado.")

        body = json.dumps(response, ensure_ascii=False).encode("utf-8")
        self.channel.basic_publish(
            exchange=self.config.exchange,
            routing_key=self.config.response_routing_key,
            body=body,
            properties=pika.BasicProperties(
                content_type="application/json",
                delivery_mode=2,
            ),
        )

    def on_message(self, _ch, method, _props, body) -> None:
        try:
            payload = json.loads(body.decode("utf-8"))
            LOGGER.info("Mensagem recebida: %s", payload)

            response = self.analyzer.predict(payload)
            self.publish_response(response)

            LOGGER.info("Mensagem de resposta publicada: %s", response)
        except Exception as exc:
            LOGGER.exception("Erro ao processar mensagem: %s", exc)
        finally:
            self.channel.basic_ack(delivery_tag=method.delivery_tag)

    def start(self) -> None:
        if self.channel is None:
            self.connect()

        self.channel.basic_consume(
            queue=self.config.consume_queue,
            on_message_callback=self.on_message,
            auto_ack=False,
        )

        LOGGER.info("Aguardando mensagens...")
        self.channel.start_consuming()


def main() -> None:
    classifier_path = os.getenv("CLASSIFIER_MODEL_PATH", DEFAULT_CLASSIFIER_PATH)
    regressor_path = os.getenv("REGRESSOR_MODEL_PATH", DEFAULT_REGRESSOR_PATH)

    config = RabbitConfig()
    analyzer = TwoModelAnalyzer(
        classifier_path=classifier_path,
        regressor_path=regressor_path,
    )
    service = RabbitMqAiService(config=config, analyzer=analyzer)
    service.start()


if __name__ == "__main__":
    main()
