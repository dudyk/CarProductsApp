namespace CarProduct.Client.Models
{
    public class GetProductsPageRequest
    {
        public string StockType { get; set; }
        public string Make { get; set; }
        public string Model { get; set; }
        public decimal? Price { get; set; }
        public double DistanceMiles { get; set; }
        public int? Zip { get; set; }
        public int PageNumber { get; set; }
    }
}