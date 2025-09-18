using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace PromoCodeFactory.DataAccess.Repositories
{
    public class EfRepository<T> : IRepository<T> where T : BaseEntity
    {
        private readonly DataContext _dbContext;
        private DbSet<T> Set => _dbContext.Set<T>();

        public EfRepository(DataContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<T> CreateAsync(T entity)
        {
            if (entity.Id == Guid.Empty)
                entity.Id = Guid.NewGuid();

            await Set.AddAsync(entity);
            await _dbContext.SaveChangesAsync();
            return entity;
        }

        public async Task<bool> DeleteAsync(T entity)
        {
            Set.Remove(entity);
            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate)
        {
            return await Set.AsNoTracking().Where(predicate).ToListAsync();
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await Set.AsNoTracking().ToListAsync();
        }

        public async Task<T> GetByIdAsync(Guid id)
        {
            return await Set.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<T> UpdateAsync(T entity)
        {
            var existing = await Set.FirstOrDefaultAsync(e => e.Id == entity.Id);
            if (existing == null)
                throw new InvalidOperationException($"{typeof(T).Name} {entity.Id} not found.");

            // Переносим простые значения
            _dbContext.Entry(existing).CurrentValues.SetValues(entity);

            // Коллекции / навигации специфичны и должны обновляться в сервисах (не обобщаем здесь)

            await _dbContext.SaveChangesAsync();
            return existing;
        }

        // -------- Advanced API --------
        public async Task<List<T>> ListAsync(
            Expression<Func<T, bool>> predicate = null,
            Func<IQueryable<T>, IQueryable<T>> include = null,
            bool asNoTracking = true)
        {
            IQueryable<T> query = Set;
            if (asNoTracking) query = query.AsNoTracking();
            if (include != null) query = include(query);
            if (predicate != null) query = query.Where(predicate);
            return await query.ToListAsync();
        }

        public async Task<T> GetSingleAsync(
            Expression<Func<T, bool>> predicate,
            Func<IQueryable<T>, IQueryable<T>> include = null,
            bool asNoTracking = true)
        {
            IQueryable<T> query = Set;
            if (asNoTracking) query = query.AsNoTracking();
            if (include != null) query = include(query);
            return await query.FirstOrDefaultAsync(predicate);
        }

        public async Task<T> GetByIdAsync(
            Guid id,
            Func<IQueryable<T>, IQueryable<T>> include,
            bool asNoTracking = true)
        {
            return await GetSingleAsync(e => e.Id == id, include, asNoTracking);
        }
    }
}
