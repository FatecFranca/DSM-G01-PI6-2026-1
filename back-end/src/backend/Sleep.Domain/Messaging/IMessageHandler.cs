namespace Sleep.Domain.Messaging
{
    public interface IMessageHandler<T>
    {
        Task HandleAsync(T  message, CancellationToken cancellationToken);
    }
}
