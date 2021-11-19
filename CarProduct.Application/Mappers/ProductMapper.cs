using CarProduct.Client.Models;
using CarProduct.Persistence.Models;

namespace CarProduct.Application.Mappers
{
    public class ProductMapper : AppMapperBase
    {
        public ProductMapper()
        {
            CreateMap<ProductSnapshot, Product>();
        }
    }
}
