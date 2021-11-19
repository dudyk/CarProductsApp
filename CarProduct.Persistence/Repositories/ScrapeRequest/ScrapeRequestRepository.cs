using Microsoft.Extensions.Options;

namespace CarProduct.Persistence.Repositories.ScrapeRequest
{
    public class ScrapeRequestRepository : RepositoryBase<Models.ScrapeRequest>, IScrapeRequestRepository
    {
        public ScrapeRequestRepository(IOptions<LiteDbConfig> configs)
            : base(configs)
        {
        }

        public Models.ScrapeRequest GetByIdWithProductsPage(int entityId)
        {
            using var scrapeRequestDb = new BaseCollection(Configs);

            return scrapeRequestDb.Collection
                .Query()
                .Include(r => r.ProductsPages)
                .Where(r => r.Id == entityId)
                .FirstOrDefault();
        }
    }
}
