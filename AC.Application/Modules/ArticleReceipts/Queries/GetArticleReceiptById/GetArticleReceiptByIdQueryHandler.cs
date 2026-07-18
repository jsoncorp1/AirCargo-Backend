using AC.Application.Abstractions.Messaging.Queries;
using AC.Application.Modules.ArticleReceipts.Specifications;
using AC.Domain.Modules.ArticleReceipts;
using AC.Domain.Persistence;
using AC.Domain.Results;

namespace AC.Application.Modules.ArticleReceipts.Queries.GetArticleReceiptById;

public class GetArticleReceiptByIdQueryHandler(IRepository<ArticleReceipt> repository)
    : IQueryHandler<GetArticleReceiptByIdQuery, GetArticleReceiptByIdQueryResult>
{
    public async Task<Result<GetArticleReceiptByIdQueryResult>> HandleAsync(
        GetArticleReceiptByIdQuery query, CancellationToken cancellationToken)
    {
        var receipt = await repository.GetBySpecificationAsync(
            new ArticleReceiptByIdSpecification(query.Id), cancellationToken);

        if (receipt is null)
            return Result.Fail<GetArticleReceiptByIdQueryResult>(
                "Recepción no encontrada.", "articlereceipt.notfound");

        return Result.Success(new GetArticleReceiptByIdQueryResult
        {
            Id = receipt.Id,
            ArticleId = receipt.ArticleId,
            ArticleSku = receipt.Article.Sku,
            ArticleName = receipt.Article.Name,
            Count = receipt.Count,
            CreatedAt = receipt.CreatedAt
        });
    }
}
