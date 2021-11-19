namespace CarProduct.Persistence.Repositories.ScrapeRequest
{
    public interface IScrapeRequestRepository : IRepositoryBase<Models.ScrapeRequest>
    {
        Models.ScrapeRequest GetByIdWithProductsPage(int entityId, int pageNumber);
    }
}