using System.Linq.Expressions;
using Ardalis.Specification;
using Ardalis.Specification.EntityFrameworkCore;
using AC.Domain.Common;
using AC.Domain.Persistence;
using AC.Domain.Results;
using AC.Domain.Specifications;
using Microsoft.EntityFrameworkCore;

namespace AC.Infrastructure.Persistence.EntityFramework.Repositories;

public class EfRepository<TEntity>(CoreDbContext context) : IRepository<TEntity>
    where TEntity : CoreEntity
{
    private readonly DbSet<TEntity> _dbSet = context.Set<TEntity>();
    private readonly SpecificationEvaluator _evaluator = SpecificationEvaluator.Default;

    // ---- Queryable base con soft-delete aplicado ----
    private IQueryable<TEntity> ActiveQuery() => _dbSet.Where(e => e.Active);

    public IQueryable<TEntity> GetQueryable() => ActiveQuery();

    // ---- Lecturas por specification ----
    public async Task<TEntity?> GetBySpecificationAsync(
        ISpecification<TEntity> specification, CancellationToken cancellationToken)
    {
        return await _evaluator
            .GetQuery(ActiveQuery(), specification)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<IEnumerable<TEntity>> GetListBySpecificationAsync(
        ISpecification<TEntity> specification, CancellationToken cancellationToken)
    {
        return await _evaluator
            .GetQuery(ActiveQuery(), specification)
            .ToListAsync(cancellationToken);
    }

    // ---- Escrituras (Add/Update al DbSet; NO commitean, commitea la UoW) ----
    public async Task<Result<TEntity>> SaveAsync(TEntity newEntity, CancellationToken cancellationToken)
    {
        await _dbSet.AddAsync(newEntity, cancellationToken);
        return Result.Success(newEntity);
    }

    public async Task<Result> SaveAsync(TEntity[] newEntity, CancellationToken cancellationToken)
    {
        await _dbSet.AddRangeAsync(newEntity, cancellationToken);
        return Result.Success();
    }

    public Task<Result<TEntity>> UpdateAsync(TEntity entity, CancellationToken cancellationToken)
    {
        _dbSet.Update(entity);
        return Task.FromResult(Result.Success(entity));
    }

    public Task<Result> UpdateAsync(TEntity[] entity, CancellationToken cancellationToken)
    {
        _dbSet.UpdateRange(entity);
        return Task.FromResult(Result.Success());
    }

    // ---- Paginación ----
    public async Task<PaginationResult<TEntity>> GetPaginatedAsync<TPagination>(
        TPagination pagination, CancellationToken cancellationToken)
        where TPagination : PaginationSpecification<TEntity>
    {
        IQueryable<TEntity> query = _evaluator.GetQuery(ActiveQuery(), pagination);

        int count = await query.CountAsync(cancellationToken);

        List<TEntity> data = await query
            .Skip((pagination.Page - 1) * pagination.PerPage)
            .Take(pagination.PerPage)
            .ToListAsync(cancellationToken);

        return new PaginationResult<TEntity>
        {
            Page = pagination.Page,
            PerPage = pagination.PerPage,
            Count = count,
            TotalPages = (int)Math.Ceiling(count / (double)pagination.PerPage),
            Data = data
        };
    }

    // ---- Consulta proyectada arbitraria ----
    public async Task<List<TResult>> ExecuteListQueryAsync<TResult>(
        Func<IQueryable<TEntity>, IQueryable<TResult>> queryBuilder,
        CancellationToken cancellationToken)
    {
        return await queryBuilder(ActiveQuery()).ToListAsync(cancellationToken);
    }

    // ---- BulkUpsert (ON CONFLICT) ----
    public async Task<Result> BulkUpsertAsync(
        TEntity[] entities, string pivot, CancellationToken cancellationToken)
    {
        foreach (TEntity entity in entities)
        {
            bool exists = await _dbSet
                .AnyAsync(e => e.Id == entity.Id, cancellationToken);

            if (exists) _dbSet.Update(entity);
            else await _dbSet.AddAsync(entity, cancellationToken);
        }

        return Result.Success();
    }

    // ---- Max ----
    public async Task<int?> MaxAsync(
        Expression<Func<TEntity, int?>> selector, CancellationToken cancellationToken)
    {
        return await ActiveQuery().MaxAsync(selector, cancellationToken);
    }
}