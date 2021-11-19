using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using CarProduct.Persistence.Repositories.Product;
using CarProduct.Persistence.Repositories.ProductsPage;
using CarProduct.Persistence.Repositories.ScrapeRequest;

namespace CarProduct.Persistence
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddLiteDb(this IServiceCollection services, IConfiguration configuration)
        {
            services.ConfigureDatabaseSettings(configuration);

            return services;
        }

        public static IServiceCollection AddRepositories(this IServiceCollection services)
        {
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<IProductsPageRepository, ProductsPageRepository>();
            services.AddScoped<IScrapeRequestRepository, ScrapeRequestRepository>();

            return services;
        }

        private static IServiceCollection ConfigureDatabaseSettings(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddOptions()
                .Configure<LiteDbConfig>(configuration.GetSection("LiteDbConfig"));

            return services;
        }
    }
}
