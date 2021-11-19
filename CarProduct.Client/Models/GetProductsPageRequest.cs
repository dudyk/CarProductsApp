namespace CarProduct.Client.Models
{
    public class GetProductsPageRequest
    {
        public string StockType { get; set; }
        public string Make { get; set; }
        public string Model { get; set; }
        public string Price { get; set; }
        public string DistanceMiles { get; set; }
        public string Zip { get; set; }
        public int PageCount { get; set; }
    }
}