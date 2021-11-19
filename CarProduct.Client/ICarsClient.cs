using System.Collections.Generic;
using System.Threading.Tasks;
using CarProduct.Client.Models;

namespace CarProduct.Client
{
    public interface ICarsClient
    {
        Task<IEnumerable<ProductsPageSnapshot>> GetProductsPage(GetProductsPageRequest scrapeRequest);
        Task<ProductSnapshot> GetProduct(string vehicleId);
    }
}