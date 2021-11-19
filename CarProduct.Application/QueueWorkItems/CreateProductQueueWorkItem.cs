using System.Threading.Tasks;
using CarProduct.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace CarProduct.Application.QueueWorkItems
{
    public class CreateProductQueueWorkItem
    {
        private readonly IServiceScopeFactory _scopeFactory;

        public CreateProductQueueWorkItem(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        public async Task DoWork(string vehicleId)
        {
            using var scope = _scopeFactory.CreateScope();

            var productCreateService = scope.ServiceProvider.GetRequiredService<ProductCreateService>();
            await productCreateService.StartProcessing(vehicleId);
        }
    }
}