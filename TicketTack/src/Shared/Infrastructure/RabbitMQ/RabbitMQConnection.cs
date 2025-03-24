using RabbitMQ.Client;
using Microsoft.Extensions.Configuration;
using System;
using System.Threading.Tasks;

namespace TicketTack.Shared.Infrastructure.RabbitMQ
{
    public class RabbitMQConnection : IAsyncDisposable
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

        public Task<IConnection> GetConnectionAsync()
        {
            if (_connection == null || !_connection.IsOpen)
            {
                _connection = _factory.CreateConnection();
            }
            return Task.FromResult(_connection);
        }

        public async ValueTask DisposeAsync()
        {
            if (_disposed) return;

            if (_connection != null)
            {
                _connection.Close();
                _connection.Dispose();
            }

            _disposed = true;
            await Task.CompletedTask;
        }
    }
}