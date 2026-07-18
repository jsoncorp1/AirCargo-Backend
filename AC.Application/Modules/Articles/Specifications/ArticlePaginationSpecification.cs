using AC.Domain.Modules.Articles;
using AC.Domain.Specifications;
using Ardalis.Specification;

namespace AC.Application.Modules.Articles.Specifications;

public class ArticlePaginationSpecification : PaginationSpecification<Article>
{
    public ArticlePaginationSpecification(int page, int perPage, Guid? supplierId = null)
        : base(page, perPage)
    {
        Query
            .Include(a => a.Supplier)
            .OrderBy(a => a.Name);

        if (supplierId is not null)
            Query.Where(a => a.SupplierId == supplierId);
    }
}
