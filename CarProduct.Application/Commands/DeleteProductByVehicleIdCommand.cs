using MediatR;

namespace CarProduct.Application.Commands
{
    public class DeleteProductByVehicleIdCommand : IRequest
    {
        public string VehicleId { get; }

        public DeleteProductByVehicleIdCommand(string vehicleId)
        {
            VehicleId = vehicleId;
        }
    }
}
