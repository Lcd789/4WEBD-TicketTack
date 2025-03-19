using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TicketTack.Shared.Infrastructure.MongoDB.Extensions;
using TicketTack.Shared.Infrastructure.RabbitMQ.Extensions;

namespace TicketTack.Shared.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddSharedServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddMongoDb(configuration);
            services.AddRabbitMQ();

            return services;
        }
    }
}
