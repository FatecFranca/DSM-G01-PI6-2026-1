namespace Sleep.Domain.Messaging
{
    public interface IMessageConsumer
    {
        Task StartConsumingAsync(CancellationToken ct);
    }
}
