using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain;
namespace PromoCodeFactory.DataAccess.Repositories
{
    public class InMemoryRepository<T>: IRepository<T> where T: BaseEntity
    {
        //protected IEnumerable<T> Data { get; set; }
        protected List<T> Data { get; set; }

        public InMemoryRepository(IEnumerable<T> data)
        {
            Data = data.ToList();
        }

        public Task<IEnumerable<T>> GetAllAsync()
        {
            return Task.FromResult(Data.AsEnumerable());
        }

        public Task<T> GetByIdAsync(Guid id)
        {
            return Task.FromResult(Data.FirstOrDefault(x => x.Id == id));
        }

        public Task<T> CreateAsync(T entity)
        {
            Data.Add(entity);
            return Task.FromResult(entity);
        }

        public Task<T> UpdateAsync(T entity)
        {
            T findEntity = Data.FirstOrDefault(x => x.Id == entity.Id);
            int index = Data.IndexOf(findEntity);
            Data[index] = entity;

            return Task.FromResult(entity);
        }

        public Task<bool> DeleteAsync(T entity)
        {
            Data.Remove(Data.First(p=>p.Id == entity.Id));
            return Task.FromResult(true);
        }

        public Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate)
        {
            return Task.FromResult(Data.AsQueryable().Where(predicate).AsEnumerable());
        }
    }
}