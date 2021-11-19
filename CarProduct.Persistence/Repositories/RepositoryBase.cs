using CarProduct.Persistence.Models.Interfaces;
using LiteDB;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;

namespace CarProduct.Persistence.Repositories
{
    public class RepositoryBase<TEntity> : IRepositoryBase<TEntity>
        where TEntity : IEntity
    {
        protected readonly LiteDbConfig Configs;

        public RepositoryBase(IOptions<LiteDbConfig> configs)
        {
            Configs = configs.Value;
        }

        public void Update(TEntity entity)
        {
            using var baseDb = new BaseCollection(Configs);

            baseDb.Collection.Update(entity);
        }

        public int Add(TEntity entity)
        {
            using var baseDb = new BaseCollection(Configs);

            return baseDb.Collection.Insert(entity).AsInt32;
        }

        public void Delete(TEntity entity)
        {
            Delete(entity.Id);
        }

        public void Delete(int entityId)
        {
            using var baseDb = new BaseCollection(Configs);

            baseDb.Collection.Delete(entityId);
        }

        public TEntity GetById(int entityId)
        {
            using var baseDb = new BaseCollection(Configs);

            return baseDb.Collection
                .Query()
                .Where(r => r.Id == entityId)
                .FirstOrDefault();
        }

        public IList<TEntity> GetAll()
        {
            using var baseDb = new BaseCollection(Configs);

            return baseDb.Collection
                .Query()
                .ToList();
        }

        protected class BaseCollection : IDisposable
        {
            private readonly LiteDatabase _liteDatabase;

            public ILiteCollection<TEntity> Collection { get; }

            public BaseCollection(LiteDbConfig configs)
            {
                _liteDatabase = new LiteDatabase(configs.DatabasePath);
                Collection = _liteDatabase.GetCollection<TEntity>(typeof(TEntity).Assembly.FullName);
            }

            public void Dispose()
            {
                _liteDatabase?.Dispose();
            }
        }
    }
}
