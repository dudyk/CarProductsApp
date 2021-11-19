using Microsoft.Extensions.DependencyInjection;
using CarProduct.Infrastructure.Queue;

namespace CarProduct.Infrastructure
{
    public static class ServiceCollectionExtensions
    {
        public static void AddQueueService(this IServiceCollection services)
        {
            services.AddHostedService<QueuedHostedService>();
            services.AddSingleton<IBackgroundTaskQueue, BackgroundTaskQueue>();
        }
    }
}
