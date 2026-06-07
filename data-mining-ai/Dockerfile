FROM python:3.10-slim

WORKDIR /app

COPY service/requirements-rabbitmq-service.txt ./

RUN pip install --no-cache-dir -r requirements-rabbitmq-service.txt

COPY . .

ENV RABBITMQ_HOST=rabbitmq
ENV RABBITMQ_PORT=5672
ENV RABBITMQ_USERNAME=guest
ENV RABBITMQ_PASSWORD=guest

CMD ["python", "service/rabbitmq_ai_service.py"]
