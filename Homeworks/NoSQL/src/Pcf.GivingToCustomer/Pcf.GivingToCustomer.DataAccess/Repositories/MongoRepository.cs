using MongoDB.Driver;
using Pcf.GivingToCustomer.Core.Abstractions.Repositories;
using Pcf.GivingToCustomer.Core.Domain;
using Pcf.GivingToCustomer.DataAccess.MongoDb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Pcf.GivingToCustomer.DataAccess.Repositories
{
    public class MongoRepository<T> : IRepository<T>
        where T : BaseEntity
    {
        private readonly IMongoCollection<T> _collection;

        public MongoRepository(MongoDbContext context)
        {
            var collectionName = typeof(T).Name.ToLower() + "s";
            _collection = context.GetCollection<T>(collectionName);
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _collection.Find(_ => true).ToListAsync();
        }

        public async Task<T> GetByIdAsync(Guid id)
        {
            return await _collection.Find(x => x.Id == id).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<T>> GetRangeByIdsAsync(List<Guid> ids)
        {
            var filter = Builders<T>.Filter.In(x => x.Id, ids);
            return await _collection.Find(filter).ToListAsync();
        }

        public async Task<T> GetFirstWhere(Expression<Func<T, bool>> predicate)
        {
            return await _collection.Find(predicate).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<T>> GetWhere(Expression<Func<T, bool>> predicate)
        {
            return await _collection.Find(predicate).ToListAsync();
        }

        public async Task AddAsync(T entity)
        {
            if (entity.Id == Guid.Empty)
            {
                entity.Id = Guid.NewGuid();
            }
            await _collection.InsertOneAsync(entity);
        }

        public async Task UpdateAsync(T entity)
        {
            var filter = Builders<T>.Filter.Eq(x => x.Id, entity.Id);
            await _collection.ReplaceOneAsync(filter, entity);
        }

        public async Task DeleteAsync(T entity)
        {
            var filter = Builders<T>.Filter.Eq(x => x.Id, entity.Id);
            await _collection.DeleteOneAsync(filter);
        }
    }
}
