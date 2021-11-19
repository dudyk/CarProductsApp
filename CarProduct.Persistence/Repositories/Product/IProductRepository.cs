using System;
using System.Collections.Generic;

namespace CarProduct.Persistence.Repositories.Product
{
    public interface IProductRepository : IRepositoryBase<Models.Product>
    {
        Models.Product GetByVehicleId(string vehicleId);
        List<Models.Product> GetOutOfDate(DateTime beforeActualizeDate);
    }
}