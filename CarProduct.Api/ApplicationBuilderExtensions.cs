using FluentScheduler;
using Microsoft.AspNetCore.Builder;

namespace CarProduct.Api
{
    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseScheduler(this IApplicationBuilder app)
        {
            JobManager.Initialize();

            return app;
        }
    }
}
