using System.Collections.Generic;

namespace CarProduct.Client.Models
{
    public class ProductsPageSnapshots
    {
        public ICollection<ProductsPageSnapshot> Items { get; set; } = new List<ProductsPageSnapshot>();
    }
}