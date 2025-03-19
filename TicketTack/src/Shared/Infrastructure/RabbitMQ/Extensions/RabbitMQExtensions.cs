using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace TicketTack.Shared.Infrastructure.RabbitMQ.Extensions
{
    public static class RabbitMQExtensions
    {
        public static IServiceCollection AddRabbitMQ(this IServiceCollection services)
        {
            services.AddSingleton<RabbitMQConnection>();
            services.AddScoped<Producer>();

            return services;
        }
    }
}
