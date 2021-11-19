using CarProduct.Application.Mappers.Extensions;
using CarProduct.Client.Models;
using CarProduct.Persistence.Models;

namespace CarProduct.Application.Mappers
{
    public class ProductsPageMapper : AppMapperBase
    {
        public ProductsPageMapper()
        {
            CreateMap<ProductsPageSnapshot, ProductsPage>();
            CreateMap<ScrapeRequest, GetProductsPageRequest>()
                .Map(r => r.StockType, p => p.StockType.ToString().ToLower());
        }
    }
}
