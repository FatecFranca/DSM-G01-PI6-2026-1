using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Sleep.Domain.Messaging;
using Sleep.Infrasctructure.Messaging.Connection;
using Sleep.Infrasctructure.Messaging.Consumers;
using System.Text;
using System.Text.Json;

namespace Sleep.Api.BackgroundServices
{
    public class MessageConsumerService : BackgroundService
    {

        private readonly IMessageConsumer _consumer;

        public MessageConsumerService(IMessageConsumer consumer)
        {
            _consumer = consumer;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await _consumer.StartConsumingAsync(stoppingToken);
        }

    }
}