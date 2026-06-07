
namespace Sleep.Domain.Messaging
{
    public interface IMessagePublisher
    {
        Task PublishAsync<T>(T message, string? routingKey = null, CancellationToken ct = default);
    }
}
