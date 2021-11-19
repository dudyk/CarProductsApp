using CarProduct.Persistence.Enums;
using MediatR;

namespace CarProduct.Application.Commands
{
    public class CreateProductsCommand : IRequest<int>
    {
        public StockTypes StockType { get; set; }
        public string Make { get; set; }
        public string Model { get; set; }
        public decimal? Price { get; set; }
        public double DistanceMiles { get; set; }
        public int? Zip { get; set; }
        public int PagesCountForScrape { get; set; }
    }
}
