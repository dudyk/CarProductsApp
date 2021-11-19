using CarProduct.Persistence.Enums;

namespace CarProduct.Api.Models
{
    public class CreateProductRequest
    {
        public StockTypes StockType { get; set; } = StockTypes.All;
        public string Make { get; set; }
        public string Model { get; set; }
        public decimal? Price { get; set; }
        public double? DistanceMiles { get; set; } = 20;
        public int? Zip { get; set; }
        public int PageCountForScrape { get; set; } = 2;
    }
}
