using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Sleep.Infrasctructure.Messaging.Connection
{
    public class RabbitMqConnection : IRabbitMqConnection
    {
        private readonly IConnectionFactory _factory;
        private readonly ILogger<RabbitMqConnection> _logger;
        private IConnection? _connection;
        private bool _disposed;
        private readonly SemaphoreSlim _semaphore = new(1, 1);

        public RabbitMqConnection(IConnectionFactory factory, ILogger<RabbitMqConnection> logger)
        {
            _factory = factory;
            _logger = logger;
        }

        public async Task<IConnection> GetConnectionAsync(CancellationToken ct = default)
        {
            if (_connection is { IsOpen: true })
                return _connection;

            await _semaphore.WaitAsync(ct);
            try
            {
                if (_connection is { IsOpen: true })
                    return _connection;

                _logger.LogInformation("Criando conexão com RabbitMQ...");

                try
                {
                    _connection = await _factory.CreateConnectionAsync(ct);
                    _connection.ConnectionShutdownAsync += OnConnectionShutdown;
                    _logger.LogInformation("Conexão com RabbitMQ estabelecida.");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Falha ao conectar no RabbitMQ. O serviço continuará sem mensageria.");
                    _connection = null;
                }
            }
            finally
            {
                _semaphore.Release();
            }

            return _connection!;
        }

        public async Task<IChannel?> CreateChannelAsync(CancellationToken ct = default)
        {
            try
            {
                var connection = await GetConnectionAsync(ct);

                if (connection is null || !connection.IsOpen)
                {
                    _logger.LogWarning("Não foi possível criar canal: conexão indisponível.");
                    return null;
                }

                return await connection.CreateChannelAsync(cancellationToken: ct);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao criar canal RabbitMQ.");
                return null;
            }
        }

        private Task OnConnectionShutdown(object? sender, ShutdownEventArgs e)
        {
            _logger.LogWarning("Conexão com RabbitMQ encerrada: {Reason}", e.ReplyText);
            _connection = null;
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;

            try
            {
                _connection?.CloseAsync().GetAwaiter().GetResult();
                _connection?.Dispose();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao fechar conexão com RabbitMQ.");
            }
        }
    }
}
