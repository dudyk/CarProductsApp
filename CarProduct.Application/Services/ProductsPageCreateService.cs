using AutoMapper;
using CarProduct.Application.Helpers;
using CarProduct.Application.QueueWorkItems;
using CarProduct.Client;
using CarProduct.Client.Models;
using CarProduct.Infrastructure.Queue;
using CarProduct.Persistence.Enums;
using CarProduct.Persistence.Models;
using CarProduct.Persistence.Repositories.ProductsPage;
using CarProduct.Persistence.Repositories.ScrapeRequest;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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

        public async Task StartProcessing(int scrapeRequestId)
        {
            if (scrapeRequestId == 0)
                throw new ArgumentNullException(nameof(scrapeRequestId));

            var scrapeRequest = _scrapeRequestRepository.GetByIdWithProductsPage(scrapeRequestId);
            if (scrapeRequest is null)
            {
                _logger.LogWarning($"ScrapeRequest with '{scrapeRequestId}' Id not exists");
                return;
            }

            var request = _mapper.Map<GetProductsPageRequest>(scrapeRequest);
            var productsPageSnapshots = await _carsClient.GetProductsPage(request);

            /*var productsPage = scrapeRequest.ProductsPages
                .FirstOrDefault(r => r.Order == pageCount);

            if (productsPage is null)
            {
                productsPage = new ProductsPage { Order = pageCount };
                scrapeRequest.ProductsPages.Add(productsPage);
            }

            if (productsPage.ProcessingStatus.IsAvailable())
                throw new InvalidOperationException($"ProductsPage with '{pageCount}' Number has incorrect '{productsPage.ProcessingStatus}' status");

            productsPage.ProcessingStatus = ProcessingStatus.Processing;
            _productsPageRepository.Update(productsPage);

            var productVehicleIds = new List<string>();
            try
            {
                var request = _mapper.Map<GetProductsPageRequest>(scrapeRequest);
                request.PageCount = pageCount;

                var productsPageSnapshots = await _carsClient.GetProductsPage(request);
                UpdateProductsPageModel(productsPage, productsPageSnapshots);
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
            }*/

            foreach (var item in productsPageSnapshots.Items.SelectMany(r => r.VehicleIds))
                _backgroundTaskQueue.QueueBackgroundWorkItem(async _ =>
                {
                    await _createProductQueueWorkItem.DoWork(item);
                });
        }

        private void UpdateProductsPageModel(ProductsPage productsPage, IEnumerable<ProductsPageSnapshot> productsPageSnapshots)
        {
            foreach (var item in productsPageSnapshots)
                _mapper.Map(item, productsPage);

        }
    }
}
