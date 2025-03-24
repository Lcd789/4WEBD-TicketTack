using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using System;
using System.Text.Json;
using System.Text;
using System.Threading.Tasks;

namespace TicketTack.Shared.Infrastructure.RabbitMQ
{
    public class Producer : IAsyncDisposable
    {
        private readonly RabbitMQConnection _connection;
        private readonly ILogger<Producer> _logger;
        private IModel _channel;
        private bool _disposed;

        public Producer(RabbitMQConnection connection, ILogger<Producer> logger)
        {
            _connection = connection;
            _logger = logger;
        }

        private async Task<IModel> GetChannelAsync()
        {
            if (_channel == null || _channel.IsClosed)
            {
                var connection = await _connection.GetConnectionAsync();
                _channel = connection.CreateModel();
            }
            return _channel;
        }

        public async Task PublishMessageAsync<T>(string exchange, string routingKey, T message) where T : class
        {
            var channel = await GetChannelAsync();

            // Declare exchange if needed
            channel.ExchangeDeclare(exchange, ExchangeType.Topic, durable: true);

            var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message));
            var properties = channel.CreateBasicProperties();
            properties.Persistent = true;
            properties.MessageId = Guid.NewGuid().ToString();
            properties.Timestamp = new AmqpTimestamp(DateTimeOffset.UtcNow.ToUnixTimeSeconds());

            channel.BasicPublish(
                exchange: exchange,
                routingKey: routingKey,
                basicProperties: properties,
                body: body);

            _logger.LogInformation($"Message published to {exchange} with routing key {routingKey}");
        }

        public ValueTask DisposeAsync()
        {
            if (_disposed) return ValueTask.CompletedTask;

            if (_channel != null)
            {
                _channel.Close();
                _channel.Dispose();
            }

            _disposed = true;

            return ValueTask.CompletedTask;
        }
    }
}