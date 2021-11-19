using System.Threading.Tasks;
using CarProduct.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace CarProduct.Application.QueueWorkItems
{
    public class CreateProductsPageQueueWorkItem
    {
        private readonly IServiceScopeFactory _scopeFactory;

        public CreateProductsPageQueueWorkItem(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        public async Task DoWork(int scrapeRequestId)
        {
            using var scope = _scopeFactory.CreateScope();

            var productsPageCreateService = scope.ServiceProvider.GetRequiredService<ProductsPageCreateService>();
            await productsPageCreateService.StartProcessing(scrapeRequestId);
        }
    }
}