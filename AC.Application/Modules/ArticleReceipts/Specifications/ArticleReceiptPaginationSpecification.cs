using AC.Domain.Modules.ArticleReceipts;
using AC.Domain.Specifications;
using Ardalis.Specification;

namespace AC.Application.Modules.ArticleReceipts.Specifications;

public class ArticleReceiptPaginationSpecification : PaginationSpecification<ArticleReceipt>
{
    public ArticleReceiptPaginationSpecification(
        int page, int perPage, Guid? articleId = null, Guid? supplierId = null)
        : base(page, perPage)
    {
        Query
            .Include(r => r.Article)
            .OrderByDescending(r => r.CreatedAt);

        if (articleId is not null)
            Query.Where(r => r.ArticleId == articleId);

        if (supplierId is not null)
            Query.Where(r => r.Article.SupplierId == supplierId);
    }
}
