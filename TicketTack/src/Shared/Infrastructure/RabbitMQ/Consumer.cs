using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace TicketTack.Shared.Infrastructure.RabbitMQ
{
    public abstract class Consumer<T> : BackgroundService where T : class
    {
        private readonly RabbitMQConnection _connection;
        private readonly ILogger<Consumer<T>> _logger;
        protected readonly string ExchangeName;
        protected readonly string QueueName;
        protected readonly string RoutingKey;
        private IModel _channel;

        protected Consumer(
            RabbitMQConnection connection,
            ILogger<Consumer<T>> logger,
            string exchangeName,
            string queueName,
            string routingKey)
        {
            _connection = connection;
            _logger = logger;
            ExchangeName = exchangeName;
            QueueName = queueName;
            RoutingKey = routingKey;
        }

        protected abstract Task ProcessMessageAsync(T message);

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                var connection = await _connection.GetConnectionAsync();
                _channel = connection.CreateModel();

                _channel.ExchangeDeclare(ExchangeName, ExchangeType.Topic, durable: true);
                _channel.QueueDeclare(QueueName, durable: true, exclusive: false, autoDelete: false);
                _channel.QueueBind(QueueName, ExchangeName, RoutingKey);
                _channel.BasicQos(0, 1, false);

                _logger.LogInformation($"Consumer started for queue: {QueueName}");

                var consumer = new EventingBasicConsumer(_channel);
                consumer.Received += async (sender, eventArgs) =>
                {
                    try
                    {
                        var message = Encoding.UTF8.GetString(eventArgs.Body.Span);
                        _logger.LogInformation($"Message received: {message}");

                        var messageObject = JsonSerializer.Deserialize<T>(message);
                        await ProcessMessageAsync(messageObject);

                        _channel.BasicAck(eventArgs.DeliveryTag, false);
                    }
                    catch (Exception ex)
                    {
                        _channel.BasicNack(eventArgs.DeliveryTag, false, true);
                        _logger.LogError(ex, "Error processing message");
                    }
                };

                _channel.BasicConsume(QueueName, false, consumer);

                // Keep the service running until cancelled
                await Task.Delay(Timeout.Infinite, stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error starting consumer");
            }
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            if (_channel != null && _channel.IsOpen)
            {
                _channel.Close();
                _channel.Dispose();
            }

            return base.StopAsync(cancellationToken);
        }
    }
}