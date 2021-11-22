using System.Collections.Generic;
using System.Threading.Tasks;
using CarProduct.Client.Models;

namespace CarProduct.Client
{
    public interface ICarsClient
    {
        Task<ProductsPageSnapshots> GetProductsPage(GetProductsPageRequest scrapeRequest);
        Task<ProductSnapshot> GetProduct(string vehicleId);
    }
}