using System.Linq.Expressions;
using AC.Domain.Common;
using AC.Domain.Results;
using Ardalis.Specification;

namespace AC.Domain.Persistence;

public interface IBaseRepository { }

public interface IBaseRepository<TEntity> : IBaseRepository where TEntity : Entity
{
    Task<TEntity?> GetBySpecificationAsync(ISpecification<TEntity> specification, CancellationToken cancellationToken);
    Task<IEnumerable<TEntity>> GetListBySpecificationAsync(ISpecification<TEntity> specification, CancellationToken cancellationToken);
    Task<Result<TEntity>> SaveAsync(TEntity newEntity, CancellationToken cancellationToken);
    Task<Result> SaveAsync(TEntity[] newEntity, CancellationToken cancellationToken);
    Task<Result<TEntity>> UpdateAsync(TEntity Entity, CancellationToken cancellationToken);
    Task<Result> UpdateAsync(TEntity[] Entity, CancellationToken cancellationToken);
    Task<PaginationResult<TEntity>> GetPaginatedAsync<TPagination>(TPagination pagination, CancellationToken cancellationToken) where TPagination : AC.Domain.Specifications.PaginationSpecification<TEntity>;
    Task<Result> BulkUpsertAsync(TEntity[] entities, string pivot, CancellationToken cancellationToken);
    Task<int?> MaxAsync(Expression<Func<TEntity, int?>> selector, CancellationToken cancellationToken);
}