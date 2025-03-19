using System;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using static MongoDB.Driver.WriteConcern;

namespace TicketTack.Shared.Infrastructure.RabbitMQ
{
    public class Producer : IDisposable
    {
        private readonly RabbitMQConnection _connection;
        private readonly ILogger<Producer> _logger;
        private IModel _channel;
        private bool _disposed;

        public Producer(RabbitMQConnection connection, ILogger<Producer> logger)
        {
            _connection = connection;
            _logger = logger;
            _channel = CreateChannel();
        }

        private IModel CreateChannel()
        {
            var channel = _connection.GetConnection().CreateModel();
            return channel;
        }

        public void PublishMessage<T>(string exchange, string routingKey, T message) where T : class
        {
            try
            {
                // Ensure channel is open
                if (_channel == null || _channel.IsClosed)
                {
                    _channel = CreateChannel();
                }

                // Declare exchange if it doesn't exist
                _channel.ExchangeDeclare(exchange, ExchangeType.Topic, durable: true);

                var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message));

                var properties = _channel.CreateBasicProperties();
                properties.DeliveryMode = 2; // Persistent message
                properties.MessageId = Guid.NewGuid().ToString();
                properties.Timestamp = new AmqpTimestamp(DateTimeOffset.UtcNow.ToUnixTimeSeconds());

                _channel.BasicPublish(
                    exchange: exchange,
                    routingKey: routingKey,
                    basicProperties: properties,
                    body: body);

                _logger.LogInformation($"Message published to {exchange} with routing key {routingKey}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error publishing message to {exchange} for routing key {routingKey}");
                throw;
            }
        }

        public void Dispose()
        {
            if (_disposed) return;

            _channel?.Close();
            _channel?.Dispose();
            _disposed = true;
        }
    }
}
