using Microsoft.Extensions.DependencyInjection;

namespace TicketTack.Shared.Infrastructure.RabbitMQ
{
    public static class RabbitMQExtensions
    {
        public static IServiceCollection AddRabbitMQ(this IServiceCollection services)
        {
            services.AddSingleton<RabbitMQConnection>();
            services.AddTransient<Producer>();

            return services;
        }

        public static IServiceCollection AddConsumer<TConsumer, TMessage>(this IServiceCollection services)
            where TConsumer : Consumer<TMessage>
            where TMessage : class
        {
            services.AddHostedService<TConsumer>();
            return services;
        }
    }
}