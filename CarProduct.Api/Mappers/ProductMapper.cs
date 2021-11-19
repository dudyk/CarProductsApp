using CarProduct.Api.Models;
using CarProduct.Application.Commands;

namespace CarProduct.Api.Mappers
{
    public class ProductMapper : AppMapperBase
    {
        public ProductMapper()
        {
            CreateMap<CreateProductRequest, CreateProductsCommand>();
        }
    }
}
