using Application.Repositories.Abstract;
using Domain.Abstract;
using Microsoft.EntityFrameworkCore;
using Persistance.Context;
using System.Linq.Expressions;

namespace Persistance.Repositories.Abstract
{
    public class BaseRepository<T> : IBaseRepository<T> where T : BaseEntity
    {
        protected readonly ApplicationDbContext _context;
        protected readonly DbSet<T> _dbSet;

        public BaseRepository(ApplicationDbContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        public async Task<bool> AddAsync(T entity, CancellationToken ct = default)
        {
            await _dbSet.AddAsync(entity, ct);
            return await SaveAsync(ct);
        }

        public async Task<bool> AddRangeAsync(IEnumerable<T> entities, CancellationToken ct = default)
        {
            await _dbSet.AddRangeAsync(entities, ct);
            return await SaveAsync(ct);
        }

        public async Task<bool> UpdateAsync(T entity, CancellationToken ct = default)
        {
            _dbSet.Update(entity);
            return await SaveAsync(ct);
        }

        public async Task<bool> DeleteAsync(T entity, CancellationToken ct = default)
        {
            _dbSet.Remove(entity);
            return await SaveAsync(ct);
        }

        public async Task<bool> RemoveAsync(T entity, CancellationToken ct = default)
        {
            _dbSet.Remove(entity);
            return await _context.SaveChangesAsync(hardDelete: true, ct) > 0;
        }

        public Task<bool> ActiveAsync(T entity, CancellationToken ct = default)
            => SetStatusAsync(entity, Status.Active, ct);

        public Task<bool> PassiveAsync(T entity, CancellationToken ct = default)
            => SetStatusAsync(entity, Status.Passive, ct);

        public async Task<T?> GetByIdAsync(Guid id, bool tracking = true, CancellationToken ct = default)
            => await Query(x => x.Id == id, tracking: tracking).FirstOrDefaultAsync(ct);

        public async Task<T?> GetByDefaultAsync(Expression<Func<T, bool>> exp, bool tracking = true, CancellationToken ct = default)
            => await Query(exp, tracking: tracking).FirstOrDefaultAsync(ct);

        public async Task<List<T>> GetByDefaultsAsync(Expression<Func<T, bool>> exp, bool tracking = true, CancellationToken ct = default)
            => await Query(exp, tracking: tracking).ToListAsync(ct);

        public async Task<T?> GetFirstOrDefaultAsync(
            Expression<Func<T, bool>>? filter = null,
            string? includeProperties = null,
            bool tracking = true,
            CancellationToken ct = default)
            => await Query(filter, null, includeProperties, tracking).FirstOrDefaultAsync(ct);

        public async Task<List<T>> GetAllAsync(
            Expression<Func<T, bool>>? filter = null,
            Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
            string? includeProperties = null,
            bool tracking = true,
            bool includeDeleted = false,
            CancellationToken ct = default)
            => await Query(filter, orderBy, includeProperties, tracking, includeDeleted).ToListAsync(ct);

        public IQueryable<T> Query(
            Expression<Func<T, bool>>? filter = null,
            Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
            string? includeProperties = null,
            bool tracking = true,
            bool includeDeleted = false)
        {
            IQueryable<T> query = _dbSet;

            if (includeDeleted)
                query = query.IgnoreQueryFilters();

            if (!tracking)
                query = query.AsNoTracking();

            if (filter != null)
                query = query.Where(filter);

            if (!string.IsNullOrWhiteSpace(includeProperties))
            {
                foreach (var prop in includeProperties.Split(',',
                             StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
                    query = query.Include(prop);
            }

            return orderBy != null ? orderBy(query) : query;
        }

        private async Task<bool> SetStatusAsync(T entity, Status status, CancellationToken ct)
        {
            entity.Status = status;
            _dbSet.Update(entity);
            return await SaveAsync(ct);
        }

        protected async Task<bool> SaveAsync(CancellationToken ct)
            => await _context.SaveChangesAsync(ct) > 0;
    }
}