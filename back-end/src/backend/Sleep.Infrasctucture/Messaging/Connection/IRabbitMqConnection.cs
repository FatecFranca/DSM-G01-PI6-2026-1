
using RabbitMQ.Client;

namespace Sleep.Infrasctructure.Messaging.Connection
{
    public interface IRabbitMqConnection : IDisposable
    {
        Task<IConnection> GetConnectionAsync(CancellationToken ct = default);
        Task<IChannel?> CreateChannelAsync(CancellationToken ct = default);
    }
}
