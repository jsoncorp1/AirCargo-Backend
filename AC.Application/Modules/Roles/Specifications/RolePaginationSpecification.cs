using AC.Domain.Modules.Roles;
using AC.Domain.Specifications;
using Ardalis.Specification;

namespace AC.Application.Modules.Roles.Specifications;

public class RolePaginationSpecification : PaginationSpecification<Role>
{
    public RolePaginationSpecification(int page, int perPage) : base(page, perPage)
    {
        Query.OrderBy(r => r.Name);
    }
}