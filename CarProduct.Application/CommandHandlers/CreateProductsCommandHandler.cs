using CarProduct.Application.Commands;
using CarProduct.Infrastructure.Queue;
using CarProduct.Persistence.Repositories.ScrapeRequest;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using CarProduct.Application.QueueWorkItems;
using CarProduct.Persistence.Models;

namespace CarProduct.Application.CommandHandlers
{
    public class CreateProductsCommandHandler: IRequestHandler<CreateProductsCommand, int>
    {
        private readonly IBackgroundTaskQueue _backgroundTaskQueue;
        private readonly IScrapeRequestRepository _scrapeRequestRepository;
        private readonly IMapper _mapper;
        private readonly CreateProductsPageQueueWorkItem _createProductsPageQueueWorkItem;

        public CreateProductsCommandHandler(
            IBackgroundTaskQueue backgroundTaskQueue,
            IScrapeRequestRepository scrapeRequestRepository,
            IMapper mapper,
            CreateProductsPageQueueWorkItem createProductsPageQueueWorkItem)
        {
            _backgroundTaskQueue = backgroundTaskQueue;
            _scrapeRequestRepository = scrapeRequestRepository;
            _mapper = mapper;
            _createProductsPageQueueWorkItem = createProductsPageQueueWorkItem;
        }

        public Task<int> Handle(CreateProductsCommand request, CancellationToken cancellationToken)
        {
            var scrapeRequest = _mapper.Map<ScrapeRequest>(request);
            var scrapeRequestId = _scrapeRequestRepository.Add(scrapeRequest);
            
            _backgroundTaskQueue.QueueBackgroundWorkItem(async _ =>
            {
                await _createProductsPageQueueWorkItem.DoWork(scrapeRequestId);
            });
            
            return Task.FromResult(scrapeRequestId);
        }
    }
}
