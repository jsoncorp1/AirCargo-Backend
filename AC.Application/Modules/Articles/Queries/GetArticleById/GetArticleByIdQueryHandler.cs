using AC.Application.Abstractions.Messaging.Queries;
using AC.Application.Modules.Articles.Specifications;
using AC.Domain.Modules.Articles;
using AC.Domain.Persistence;
using AC.Domain.Results;

namespace AC.Application.Modules.Articles.Queries.GetArticleById;

public class GetArticleByIdQueryHandler(IRepository<Article> repository)
    : IQueryHandler<GetArticleByIdQuery, GetArticleByIdQueryResult>
{
    public async Task<Result<GetArticleByIdQueryResult>> HandleAsync(
        GetArticleByIdQuery query, CancellationToken cancellationToken)
    {
        var article = await repository.GetBySpecificationAsync(
            new ArticleByIdSpecification(query.Id), cancellationToken);

        if (article is null)
            return Result.Fail<GetArticleByIdQueryResult>("Artículo no encontrado.", "article.notfound");

        return Result.Success(new GetArticleByIdQueryResult
        {
            Id = article.Id,
            Sku = article.Sku,
            Name = article.Name,
            Count = article.Count,
            Price = article.Price,
            SupplierId = article.SupplierId,
            SupplierName = article.Supplier.Name
        });
    }
}
