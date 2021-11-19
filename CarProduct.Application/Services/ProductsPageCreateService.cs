using CarProduct.Application.QueueWorkItems;
using CarProduct.Infrastructure.Queue;
using CarProduct.Persistence.Repositories.ScrapeRequest;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using CarProduct.Application.Helpers;
using CarProduct.Client;
using CarProduct.Client.Models;
using CarProduct.Persistence.Enums;
using CarProduct.Persistence.Models;
using CarProduct.Persistence.Repositories.ProductsPage;

namespace CarProduct.Application.Services
{
    public class ProductsPageCreateService
    {
        private readonly ILogger<ProductsPageCreateService> _logger;
        private readonly IScrapeRequestRepository _scrapeRequestRepository;
        private readonly IBackgroundTaskQueue _backgroundTaskQueue;
        private readonly CreateProductQueueWorkItem _createProductQueueWorkItem;
        private readonly IProductsPageRepository _productsPageRepository;
        private readonly ICarsClient _carsClient;
        private readonly IMapper _mapper;

        public ProductsPageCreateService(
            ILogger<ProductsPageCreateService> logger,
            IScrapeRequestRepository scrapeRequestRepository,
            IBackgroundTaskQueue backgroundTaskQueue,
            CreateProductQueueWorkItem createProductQueueWorkItem,
            IProductsPageRepository productsPageRepository,
            ICarsClient carsClient,
            IMapper mapper)
        {
            _logger = logger;
            _scrapeRequestRepository = scrapeRequestRepository;
            _backgroundTaskQueue = backgroundTaskQueue;
            _createProductQueueWorkItem = createProductQueueWorkItem;
            _productsPageRepository = productsPageRepository;
            _carsClient = carsClient;
            _mapper = mapper;
        }

        public async Task StartProcessing(int scrapeRequestId, int pageNumber)
        {
            if (scrapeRequestId == 0)
                throw new ArgumentNullException(nameof(scrapeRequestId));

            if (pageNumber == 0)
                throw new ArgumentNullException(nameof(pageNumber));

            var scrapeRequest = _scrapeRequestRepository.GetByIdWithProductsPage(scrapeRequestId, pageNumber);
            if (scrapeRequest is null)
            {
                _logger.LogWarning($"ScrapeRequest with '{scrapeRequestId}' Id not exists");
                return;
            }

            var productsPage = scrapeRequest.ProductsPages
                .FirstOrDefault(r => r.Order == pageNumber);

            if (productsPage is null)
            {
                productsPage = new ProductsPage { Order = pageNumber };
                scrapeRequest.ProductsPages.Add(productsPage);
            }

            if (productsPage.ProcessingStatus.IsAvailable())
                throw new InvalidOperationException($"ProductsPage with '{pageNumber}' Number has incorrect '{productsPage.ProcessingStatus}' status");

            productsPage.ProcessingStatus = ProcessingStatus.Processing;
            _productsPageRepository.Update(productsPage);

            var productVehicleIds = new List<string>();
            try
            {
                var request = _mapper.Map<GetProductsPageRequest>(scrapeRequest);
                request.PageNumber = pageNumber;

                var productsPageSnapshot = await _carsClient.GetProductsPage(request);
                UpdateProductsPageModel(productsPage, productsPageSnapshot);
                productVehicleIds.AddRange(productsPage.Products.Select(r => r.VehicleId));

                productsPage.ProcessingStatus = ProcessingStatus.Succeeded;
                productsPage.OnActualized = DateTime.UtcNow;
                _productsPageRepository.Update(productsPage);
            }
            catch (Exception exception)
            {
                productsPage.ProcessingStatus = ProcessingStatus.Failed;
                _productsPageRepository.Update(productsPage);

                _logger.LogError(exception, $"{nameof(StartProcessing)} {nameof(ProductsPage)} is failed");
            }

            _backgroundTaskQueue.QueueBackgroundWorkItem(async _ =>
            {
                foreach (var item in productVehicleIds)
                    await _createProductQueueWorkItem.DoWork(item);
            });
        }

        private void UpdateProductsPageModel(ProductsPage productsPage, ProductsPageSnapshot productsPageSnapshot)
        {
            _mapper.Map(productsPageSnapshot, productsPage);
        }
    }
}
