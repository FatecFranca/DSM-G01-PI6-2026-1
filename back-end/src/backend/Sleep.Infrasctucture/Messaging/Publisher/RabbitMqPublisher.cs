using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using Sleep.Domain.Messaging;
using Sleep.Infrasctructure.Messaging.Connection;
using System.Text;
using System.Text.Json;

namespace Sleep.Infrasctructure.Messaging.Publisher
{
    public class RabbitMqPublisher : IMessagePublisher
    {
        private readonly IRabbitMqConnection _connection;
        private readonly IConfiguration _config;
        private readonly ILogger<RabbitMqPublisher> _logger;

        private readonly string _exchange;
        private readonly string _defaultRoutingKey;

        public RabbitMqPublisher(
            IRabbitMqConnection connection,
            IConfiguration config,
            ILogger<RabbitMqPublisher> logger)
        {
            _connection = connection;
            _config = config;
            _logger = logger;

            _exchange = config["RabbitMQ:ExchangeName"]!;
            _defaultRoutingKey = config["RabbitMQ:PublishRoutingKey"]!;
        }

        public async Task PublishAsync<T>(T message, string? routingKey = null, CancellationToken ct = default)
        {
            var key = routingKey ?? _defaultRoutingKey;
            var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message));

            var channel = await _connection.CreateChannelAsync(ct);

            if (channel is null)
            {
                _logger.LogWarning("Publicação ignorada: canal RabbitMQ indisponível.");
                return;
            }

            await using (channel)
            {
                await channel.ExchangeDeclareAsync(
                    exchange: _exchange,
                    type: ExchangeType.Topic,
                    durable: true,
                    autoDelete: false,
                    cancellationToken: ct);

                var props = new BasicProperties
                {
                    ContentType = "application/json",
                    DeliveryMode = DeliveryModes.Persistent
                };

                await channel.BasicPublishAsync(
                    exchange: _exchange,
                    routingKey: key,
                    mandatory: false,
                    basicProperties: props,
                    body: body,
                    cancellationToken: ct);

                _logger.LogInformation("Mensagem publicada → exchange: {Exchange} | routing key: {Key}", _exchange, key);
            }
        }
    }
}
