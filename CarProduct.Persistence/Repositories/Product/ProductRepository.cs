using CarProduct.Persistence.Enums;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;

namespace CarProduct.Persistence.Repositories.Product
{
    public class ProductRepository : RepositoryBase<Models.Product>, IProductRepository
    {
        public ProductRepository(IOptions<LiteDbConfig> configs)
            : base(configs)
        {
        }

        public Models.Product GetByVehicleId(string vehicleId)
        {
            using var productDb = new BaseCollection(Configs);

            return productDb.Collection
                .Query()
                .Where(r => r.VehicleId
                    .Equals(vehicleId, StringComparison.InvariantCultureIgnoreCase))
                .FirstOrDefault();
        }

        public List<Models.Product> GetOutOfDate(DateTime beforeActualizeDate)
        {
            using var productDb = new BaseCollection(Configs);

            return productDb.Collection
                .Query()
                .Where(r => (r.OnActualized <= beforeActualizeDate
                             || r.OnActualized == null)
                            && (r.ProcessingStatus == ProcessingStatus.Succeeded
                                || r.ProcessingStatus == ProcessingStatus.Failed))
                .ToList();
        }
    }
}
