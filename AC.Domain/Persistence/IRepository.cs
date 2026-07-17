using AC.Domain.Common;

namespace AC.Domain.Persistence;

public interface IRepository<TEntity> : IBaseRepository<TEntity>
    where TEntity : CoreEntity
{
    IQueryable<TEntity> GetQueryable();
    
    Task<List<TResult>> ExecuteListQueryAsync<TResult>(
        Func<IQueryable<TEntity>, IQueryable<TResult>> queryBuilder,
        CancellationToken cancellationToken);
}
