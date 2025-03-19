using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;

namespace TicketTack.Shared.Infrastructure.RabbitMQ
{
    public class RabbitMQConnection
    {
        private readonly ConnectionFactory _factory;
        private IConnection _connection;
        private bool _disposed;

        public RabbitMQConnection(IConfiguration configuration)
        {
            _factory = new ConnectionFactory
            {
                HostName = configuration["RabbitMQ:HostName"],
                UserName = configuration["RabbitMQ:UserName"],
                Password = configuration["RabbitMQ:Password"],
                Port = int.Parse(configuration["RabbitMQ:Port"])
            };
        }

        public IConnection GetConnection()
        {
            if (_connection == null || !_connection.IsOpen)
            {
                _connection = _factory.CreateConnection();
            }

            return _connection;
        }

        public void Dispose()
        {
            if (_disposed) return;

            _connection?.Dispose();
            _disposed = true;
        }
    }
}
