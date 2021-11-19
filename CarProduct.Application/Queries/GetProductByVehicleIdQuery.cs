using MediatR;
using CarProduct.Persistence.Models;

namespace CarProduct.Application.Queries
{
    public class GetProductByVehicleIdQuery : IRequest<Product>
    {
        public string VehicleId { get; }

        public GetProductByVehicleIdQuery(string vehicleId)
        {
            VehicleId = vehicleId;
        }
    }
}
