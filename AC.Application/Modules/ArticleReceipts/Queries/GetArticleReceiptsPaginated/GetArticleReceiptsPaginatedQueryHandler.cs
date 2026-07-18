using AC.Application.Abstractions.Messaging.Queries;
using AC.Application.Modules.ArticleReceipts.Specifications;
using AC.Domain.Modules.ArticleReceipts;
using AC.Domain.Persistence;
using AC.Domain.Results;

namespace AC.Application.Modules.ArticleReceipts.Queries.GetArticleReceiptsPaginated;

public class GetArticleReceiptsPaginatedQueryHandler(IRepository<ArticleReceipt> repository)
    : IQueryHandler<GetArticleReceiptsPaginatedQuery, GetArticleReceiptsPaginatedQueryResult>
{
    public async Task<Result<GetArticleReceiptsPaginatedQueryResult>> HandleAsync(
        GetArticleReceiptsPaginatedQuery query, CancellationToken cancellationToken)
    {
        int page = query.Page < 1 ? 1 : query.Page;
        int perPage = query.PerPage is < 1 or > 100 ? 10 : query.PerPage;

        var spec = new ArticleReceiptPaginationSpecification(page, perPage, query.ArticleId);
        var result = await repository.GetPaginatedAsync(spec, cancellationToken);

        return Result.Success(new GetArticleReceiptsPaginatedQueryResult
        {
            Page = result.Page,
            PerPage = result.PerPage,
            TotalPages = result.TotalPages,
            Count = result.Count,
            Data = result.Data.Select(r => new ArticleReceiptPaginatedItem
            {
                Id = r.Id,
                ArticleId = r.ArticleId,
                ArticleSku = r.Article.Sku,
                ArticleName = r.Article.Name,
                Count = r.Count,
                CreatedAt = r.CreatedAt
            })
        });
    }
}
