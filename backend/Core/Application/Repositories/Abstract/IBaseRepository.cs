using Domain.Abstract;
using System.Linq.Expressions;

namespace Application.Repositories.Abstract
{
    public interface IBaseRepository<T> where T : BaseEntity
    {
        Task<bool> AddAsync(T entity, CancellationToken ct = default);
        Task<bool> AddRangeAsync(IEnumerable<T> entities, CancellationToken ct = default);
        Task<bool> UpdateAsync(T entity, CancellationToken ct = default);
        Task<bool> DeleteAsync(T entity, CancellationToken ct = default);
        Task<bool> RemoveAsync(T entity, CancellationToken ct = default);
        Task<bool> ActiveAsync(T entity, CancellationToken ct = default);
        Task<bool> PassiveAsync(T entity, CancellationToken ct = default);

        Task<T?> GetByIdAsync(Guid id, bool tracking = true, CancellationToken ct = default);

        Task<T?> GetByDefaultAsync(Expression<Func<T, bool>> exp, bool tracking = true, CancellationToken ct = default);
        Task<List<T>> GetByDefaultsAsync(Expression<Func<T, bool>> exp, bool tracking = true, CancellationToken ct = default);

        Task<T?> GetFirstOrDefaultAsync(
            Expression<Func<T, bool>>? filter = null,
            string? includeProperties = null,
            bool tracking = true,
            CancellationToken ct = default);

        Task<List<T>> GetAllAsync(
            Expression<Func<T, bool>>? filter = null,
            Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
            string? includeProperties = null,
            bool tracking = true,
            bool includeDeleted = false,
            CancellationToken ct = default);

        IQueryable<T> Query(
            Expression<Func<T, bool>>? filter = null,
            Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
            string? includeProperties = null,
            bool tracking = true,
            bool includeDeleted = false);
    }
}