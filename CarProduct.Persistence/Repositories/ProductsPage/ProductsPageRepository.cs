using Microsoft.Extensions.Options;

namespace CarProduct.Persistence.Repositories.ProductsPage
{
    public class ProductsPageRepository : RepositoryBase<Models.ProductsPage>, IProductsPageRepository
    {
        public ProductsPageRepository(IOptions<LiteDbConfig> configs)
            : base(configs)
        {
        }
    }
}
