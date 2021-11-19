using CarProduct.Application.Services;
using CarProduct.Application.Settings;
using CarProduct.Application.UserNotification;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using CarProduct.Application.QueueWorkItems;
using CarProduct.Client;
using MediatR;

namespace CarProduct.Application
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddAutoMapper(typeof(Mappers.AppMapperBase));
            services.AddMediatR(typeof(ServiceCollectionExtensions).Assembly);

            AddCarsClient(services, configuration);
            AddUserNotification(services, configuration);

            services.AddScoped<ProductCreateService, ProductCreateService>();
            services.AddScoped<ProductsPageCreateService, ProductsPageCreateService>();

            services.AddTransient<CreateProductQueueWorkItem, CreateProductQueueWorkItem>();
            services.AddTransient<CreateProductsPageQueueWorkItem, CreateProductsPageQueueWorkItem>();

            return services;
        }

        private static IServiceCollection AddCarsClient(IServiceCollection services, IConfiguration configuration)
        {
            services.AddOptions()
                .Configure<CarsClientSettings>(configuration.GetSection("CarsClientSettings"));

            var settings = configuration.GetSection("CarsClientSettings").Get<CarsClientSettings>();

            services.AddSingleton<ICarsClient>((_) => new CarsClient(settings.Url, settings.UserName, settings.Password));

            return services;
        }

        private static IServiceCollection AddUserNotification(IServiceCollection services, IConfiguration configuration)
        {
            services.AddOptions()
                .Configure<EmailNotificationSettings>(configuration.GetSection("EmailNotificationSettings"))
                .Configure<SmtpSettings>(configuration.GetSection("SmtpSettings"));
            services.AddScoped<IUserNotification, EmailNotification>();

            return services;
        }
    }
}
