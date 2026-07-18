using AC.Domain.Modules.Articles;
using Ardalis.Specification;

namespace AC.Application.Modules.Articles.Specifications;

public class ArticleBySkuSpecification : Specification<Article>
{
    public ArticleBySkuSpecification(string sku) => Query.Where(a => a.Sku == sku);
}
