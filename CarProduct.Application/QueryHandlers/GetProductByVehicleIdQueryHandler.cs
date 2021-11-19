using CarProduct.Application.Queries;
using CarProduct.Persistence.Models;
using CarProduct.Persistence.Repositories.Product;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace CarProduct.Application.QueryHandlers
{
    public class GetProductByVehicleIdQueryHandler : IRequestHandler<GetProductByVehicleIdQuery, Product>
    {
        private readonly IProductRepository _productRepository;

        public GetProductByVehicleIdQueryHandler(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public Task<Product> Handle(GetProductByVehicleIdQuery request, CancellationToken cancellationToken)
        {
            return Task.FromResult(_productRepository.GetByVehicleId(request.VehicleId));
        }
    }
}
