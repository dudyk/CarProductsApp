using CarProduct.Application.Notifications;
using CarProduct.Persistence.Enums;
using CarProduct.Persistence.Models;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using CarProduct.Application.Helpers;
using CarProduct.Client;
using CarProduct.Client.Models;
using CarProduct.Persistence.Repositories.Product;

namespace CarProduct.Application.Services
{
    public class ProductCreateService
    {
        private readonly ILogger<ProductCreateService> _logger;
        private readonly IMediator _mediator;
        private readonly IProductRepository _productRepository;
        private readonly ICarsClient _carsClient;
        private readonly IMapper _mapper;

        public ProductCreateService(
            ILogger<ProductCreateService> logger,
            IMediator mediator,
            IProductRepository productRepository,
            ICarsClient carsClient,
            IMapper mapper)
        {
            _logger = logger;
            _mediator = mediator;
            _productRepository = productRepository;
            _carsClient = carsClient;
            _mapper = mapper;
        }

        public async Task StartProcessing(string vehicleId)
        {
            if (string.IsNullOrWhiteSpace(vehicleId))
                throw new ArgumentNullException(nameof(vehicleId));

            var product = _productRepository.GetByVehicleId(vehicleId)
                          ?? new Product();

            if (product.ProcessingStatus.IsAvailable())
                throw new InvalidOperationException($"Product with '{vehicleId}' VehicleId has incorrect '{product.ProcessingStatus}' status");

            product.ProcessingStatus = ProcessingStatus.Processing;
            _productRepository.Update(product);

            try
            {
                var productSnapshot = await _carsClient.GetProduct(vehicleId);
                UpdateProductModel(product, productSnapshot);

                product.ProcessingStatus = ProcessingStatus.Succeeded;
                product.OnActualized = DateTime.UtcNow;
                _productRepository.Update(product);
            }
            catch(Exception ex)
            {
                product.ProcessingStatus = ProcessingStatus.Failed;
                _productRepository.Update(product);

                _logger.LogError(ex, $"{nameof(StartProcessing)} {nameof(Product)} is failed");
            }

            //TODO it would be better use queue instead of mediator or no waiting publish event at least
            //also we need to publish unsuccessful notification
            if (product.ProcessingStatus == ProcessingStatus.Succeeded)
                await _mediator.Publish(new ProductCreatedNotification(product.VehicleId), CancellationToken.None);
        }

        private void UpdateProductModel(Product product, ProductSnapshot productSnapshot)
        {
            _mapper.Map(productSnapshot, product);
        }
    }
}
