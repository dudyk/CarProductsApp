using CarProduct.Persistence.Enums;
using CarProduct.Persistence.Models.Interfaces;
using System.Collections.Generic;

namespace CarProduct.Persistence.Models
{
    public class ScrapeRequest : IEntity
    {
        public int Id { get; set; }
        public StockTypes StockType { get; set; }
        public string Make { get; set; }
        public string Model { get; set; }
        public decimal? Price { get; set; }
        public double DistanceMiles { get; set; }
        public int? Zip { get; set; }
        public int PagesCountForScrape { get; set; }
        
        public ICollection<ProductsPage> ProductsPages { get; set; } = new HashSet<ProductsPage>();
    }
}
