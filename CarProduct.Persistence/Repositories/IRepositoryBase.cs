using System.Collections.Generic;

namespace CarProduct.Persistence.Repositories
{
    public interface IRepositoryBase<TEntity>
    {
        void Update(TEntity entity);
        int Add(TEntity entity);
        void Delete(TEntity entity);
        void Delete(int entityId);
        TEntity GetById(int entityId);
        IList<TEntity> GetAll();
    }
}