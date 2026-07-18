using AC.Domain.Modules.Articles;
using Ardalis.Specification;

namespace AC.Application.Modules.Articles.Specifications;

public class ArticleByIdSpecification : Specification<Article>
{
    public ArticleByIdSpecification(Guid id)
    {
        Query
            .Where(a => a.Id == id)
            .Include(a => a.Supplier);
    }
}
