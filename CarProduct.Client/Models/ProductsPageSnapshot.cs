using System.Collections.Generic;

namespace CarProduct.Client.Models
{
    public class ProductsPageSnapshot
    {
        public int PageNumber { get; set; }
        public string ScreenShotFileName { get; set; }
        public IEnumerable<string> VehicleIds { get; set; } = new List<string>();
    }
}
