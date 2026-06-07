using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;
using Sleep.Domain.Messaging;
using Sleep.Infrasctructure.Messaging.Connection;
using Sleep.Infrasctructure.Messaging.Consumers;
using Sleep.Infrasctructure.Messaging.Publisher;

namespace Sleep.Infrasctructure.Messaging.Extensions
{
    public static class RabbitMqExtensions
    {
        public static void AddRabbitMq(IServiceCollection services, IConfiguration configuration)
        {
            var section = configuration.GetSection("RabbitMQ");

            services.AddSingleton<IConnectionFactory>(_ => new ConnectionFactory
            {
                HostName = section["Host"]!,
                Port = int.Parse(section["Port"]!),
                UserName = section["Username"]!,
                Password = section["Password"]!,
                VirtualHost = section["VirtualHost"]!,
                AutomaticRecoveryEnabled = true,
                NetworkRecoveryInterval = TimeSpan.FromSeconds(10)
            });

            services.AddSingleton<IRabbitMqConnection, RabbitMqConnection>();
            services.AddSingleton<IMessagePublisher, RabbitMqPublisher>();
            services.AddSingleton<IMessageConsumer, RabbitMqConsumer>();
        }
    }
}
