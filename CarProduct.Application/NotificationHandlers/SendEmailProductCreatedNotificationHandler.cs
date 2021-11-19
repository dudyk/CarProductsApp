using CarProduct.Application.Notifications;
using CarProduct.Application.UserNotification;
using CarProduct.Persistence.Repositories.Product;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace CarProduct.Application.NotificationHandlers
{
    public class SendEmailProductCreatedNotificationHandler : INotificationHandler<ProductCreatedNotification>
    {
        private readonly IUserNotification _userNotification;
        private readonly IProductRepository _productRepository;

        public SendEmailProductCreatedNotificationHandler(
            IUserNotification userNotification,
            IProductRepository productRepository)
        {
            _userNotification = userNotification;
            _productRepository = productRepository;
        }

        public async Task Handle(ProductCreatedNotification notification, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(notification.VehicleId))
                throw new ArgumentNullException(nameof(notification.VehicleId));

            var product = _productRepository.GetByVehicleId(notification.VehicleId);
            if (product is null)
                throw new InvalidOperationException($"Product with '{notification.VehicleId}' VehicleId not exists");

            await _userNotification.NotifyCreateProduct(product);
        }
    }
}
