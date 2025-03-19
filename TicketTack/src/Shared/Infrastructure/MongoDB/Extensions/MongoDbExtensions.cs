using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace TicketTack.Shared.Infrastructure.MongoDB.Extensions
{
    public static class MongoDbExtensions
    {
        public static IServiceCollection AddMongoDb(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<MongoDbContext>();

            return services;
        }

        public static IServiceCollection AddMongoRepository<T>(this IServiceCollection services, string collectionName) where T : Models.BaseEntity
        {
            services.AddScoped<IMongoRepository<T>>(provider =>
                new MongoRepository<T>(provider.GetRequiredService<MongoDbContext>(), collectionName));

            return services;
        }
    }
}
