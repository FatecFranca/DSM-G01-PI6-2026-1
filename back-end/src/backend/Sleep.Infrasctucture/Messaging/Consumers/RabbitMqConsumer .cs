using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Sleep.Communication.Events;
using Sleep.Domain.Messaging;
using Sleep.Infrasctructure.Messaging.Connection;
using System.Text;
using System.Text.Json;

namespace Sleep.Infrasctructure.Messaging.Consumers;

public class RabbitMqConsumer : IMessageConsumer
{
    private readonly IRabbitMqConnection _connection;
    private readonly ILogger<RabbitMqConsumer> _logger;
    private readonly IServiceScopeFactory _scopeFactory;

    private readonly string _exchange;
    private readonly string _queue;
    private readonly string _routingKey;

    public RabbitMqConsumer(
        IRabbitMqConnection connection,
        IConfiguration config,
        ILogger<RabbitMqConsumer> logger,
        IServiceScopeFactory scopeFactory)
    {
        _connection = connection;
        _logger = logger;
        _scopeFactory = scopeFactory;

        _exchange = config["RabbitMQ:ExchangeName"]!;
        _queue = config["RabbitMQ:ConsumeQueue"]!;
        _routingKey = config["RabbitMQ:ConsumeRoutingKey"]!;
    }

    public async Task StartConsumingAsync(CancellationToken ct = default)
    {
        var channel = await _connection.CreateChannelAsync(ct);

        if (channel is null)
        {
            _logger.LogWarning("Canal RabbitMQ indisponível. Consumer não iniciado.");
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

            await channel.QueueDeclareAsync(
                queue: _queue,
                durable: true,
                exclusive: false,
                autoDelete: false,
                cancellationToken: ct);

            await channel.QueueBindAsync(
                queue: _queue,
                exchange: _exchange,
                routingKey: _routingKey,
                cancellationToken: ct);

            await channel.BasicQosAsync(
                prefetchSize: 0,
                prefetchCount: 1,
                global: false,
                cancellationToken: ct);

            var consumer = new AsyncEventingBasicConsumer(channel);

            consumer.ReceivedAsync += async (_, ea) =>
            {
                try
                {
                    var body = Encoding.UTF8.GetString(ea.Body.ToArray());
                    _logger.LogInformation("Mensagem recebida do Python: {Body}", body);

                    var resultado = JsonSerializer.Deserialize<SleepAnalysisResponseMessage>(body);

                    await ProcessarRetornoAsync(resultado, ct);

                    await channel.BasicAckAsync(ea.DeliveryTag, multiple: false, cancellationToken: ct);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Erro ao processar mensagem.");
                    await channel.BasicAckAsync(ea.DeliveryTag, multiple: false, cancellationToken: ct);
                }
            };

            await channel.BasicConsumeAsync(
                queue: _queue,
                autoAck: false,
                consumer: consumer,
                cancellationToken: ct);

            await Task.Delay(Timeout.Infinite, ct);
        } 
    }

    private async Task ProcessarRetornoAsync(SleepAnalysisResponseMessage? data, CancellationToken ct)
    {
        if (data is null) return;

        await using var scope = _scopeFactory.CreateAsyncScope();

        var handler = scope.ServiceProvider.GetRequiredService<IMessageHandler<SleepAnalysisResponseMessage>>();

        await handler.HandleAsync(data, ct);
    }
}