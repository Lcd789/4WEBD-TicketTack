using System;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using static MongoDB.Driver.WriteConcern;

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

        protected Consumer(RabbitMQConnection connection, ILogger<Consumer<T>> logger,
            string exchangeName, string queueName, string routingKey)
        {
            _connection = connection;
            _logger = logger;
            ExchangeName = exchangeName;
            QueueName = queueName;
            RoutingKey = routingKey;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();

            _channel = _connection.GetConnection().CreateModel();

            _channel.ExchangeDeclare(ExchangeName, ExchangeType.Topic, durable: true);
            _channel.QueueDeclare(QueueName, durable: true, exclusive: false, autoDelete: false);
            _channel.QueueBind(QueueName, ExchangeName, RoutingKey);

            _channel.BasicQos(0, 1, false);

            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += async (sender, eventArgs) =>
            {
                var body = eventArgs.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);

                try
                {
                    _logger.LogInformation($"Received message: {message}");
                    var messageObject = JsonSerializer.Deserialize<T>(message);

                    await ProcessMessageAsync(messageObject);

                    _channel.BasicAck(eventArgs.DeliveryTag, multiple: false);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Error processing message: {message}");
                    _channel.BasicNack(eventArgs.DeliveryTag, multiple: false, requeue: true);
                }
            };

            _channel.BasicConsume(QueueName, autoAck: false, consumer);

            return Task.CompletedTask;
        }

        protected abstract Task ProcessMessageAsync(T message);

        public override void Dispose()
        {
            _channel?.Close();
            _channel?.Dispose();
            base.Dispose();
        }
    }
}
