using MediatR;

namespace CarProduct.Application.Notifications
{
    public class ProductCreatedNotification : INotification
    {
        public string VehicleId { get; }

        public ProductCreatedNotification(string vehicleId)
        {
            VehicleId = vehicleId;
        }
    }
}
