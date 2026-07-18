using AC.Domain.Modules.ArticleReceipts;
using Ardalis.Specification;

namespace AC.Application.Modules.ArticleReceipts.Specifications;

public class ArticleReceiptByIdSpecification : Specification<ArticleReceipt>
{
    public ArticleReceiptByIdSpecification(Guid id)
    {
        Query
            .Where(r => r.Id == id)
            .Include(r => r.Article);
    }
}
