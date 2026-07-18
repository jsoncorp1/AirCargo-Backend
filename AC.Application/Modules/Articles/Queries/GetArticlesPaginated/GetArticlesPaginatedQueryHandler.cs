using AC.Application.Abstractions.Messaging.Queries;
using AC.Application.Modules.Articles.Specifications;
using AC.Domain.Modules.Articles;
using AC.Domain.Persistence;
using AC.Domain.Results;

namespace AC.Application.Modules.Articles.Queries.GetArticlesPaginated;

public class GetArticlesPaginatedQueryHandler(IRepository<Article> repository)
    : IQueryHandler<GetArticlesPaginatedQuery, GetArticlesPaginatedQueryResult>
{
    public async Task<Result<GetArticlesPaginatedQueryResult>> HandleAsync(
        GetArticlesPaginatedQuery query, CancellationToken cancellationToken)
    {
        int page = query.Page < 1 ? 1 : query.Page;
        int perPage = query.PerPage is < 1 or > 100 ? 10 : query.PerPage;

        var spec = new ArticlePaginationSpecification(page, perPage, query.SupplierId);
        var result = await repository.GetPaginatedAsync(spec, cancellationToken);

        return Result.Success(new GetArticlesPaginatedQueryResult
        {
            Page = result.Page,
            PerPage = result.PerPage,
            TotalPages = result.TotalPages,
            Count = result.Count,
            Data = result.Data.Select(a => new ArticlePaginatedItem
            {
                Id = a.Id,
                Sku = a.Sku,
                Name = a.Name,
                Count = a.Count,
                Price = a.Price,
                SupplierId = a.SupplierId,
                SupplierName = a.Supplier.Name
            })
        });
    }
}
