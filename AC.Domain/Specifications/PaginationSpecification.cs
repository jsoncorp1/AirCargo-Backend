using AC.Domain.Common;
using Ardalis.Specification;

namespace AC.Domain.Specifications;

public class PaginationSpecification<TEntity> : Specification<TEntity> where TEntity : Entity
{
    public int Page { get; private set; }
    public int PerPage { get; private set; }

    public PaginationSpecification(int Page, int PerPage)
    {
        this.Page = Page;
        this.PerPage = PerPage;
    }
}
