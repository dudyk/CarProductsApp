using CarProduct.Application.Commands;
using CarProduct.Persistence.Repositories.Product;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace CarProduct.Application.CommandHandlers
{
    public class DeleteProductByVehicleIdCommandHandler : IRequestHandler<DeleteProductByVehicleIdCommand>
    {
        private readonly IProductRepository _productRepository;

        public DeleteProductByVehicleIdCommandHandler(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public Task<Unit> Handle(DeleteProductByVehicleIdCommand request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.VehicleId))
                throw new ArgumentNullException(nameof(request.VehicleId));

            var product = _productRepository.GetByVehicleId(request.VehicleId);
            if (product is null)
                throw new InvalidOperationException($"Product with '{request.VehicleId}' VehicleId not found");

            _productRepository.Delete(product);

            return Task.FromResult(new Unit());
        }
    }
}
