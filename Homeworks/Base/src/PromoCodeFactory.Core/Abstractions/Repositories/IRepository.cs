using PromoCodeFactory.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace PromoCodeFactory.Core.Abstractions.Repositories
{
    public interface IRepository<T> where T: BaseEntity
    {
        Task<IEnumerable<T>> GetAllAsync();
        Task<T> GetByIdAsync(Guid id);
        Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);
        Task<T> CreateAsync(T entity);
        Task<T> UpdateAsync(T entity);
        Task<bool> DeleteAsync(T entity);

        // Новые расширенные методы:

        // Универсальный список с возможностью указать предикат и include-цепочку
        Task<List<T>> ListAsync(
            Expression<Func<T, bool>> predicate = null,
            Func<IQueryable<T>, IQueryable<T>> include = null,
            bool asNoTracking = true);

        // Один объект по условию
        Task<T> GetSingleAsync(
            Expression<Func<T, bool>> predicate,
            Func<IQueryable<T>, IQueryable<T>> include = null,
            bool asNoTracking = true);

        // Перегрузка GetById с include
        Task<T> GetByIdAsync(
            Guid id,
            Func<IQueryable<T>, IQueryable<T>> include,
            bool asNoTracking = true);
    }
}