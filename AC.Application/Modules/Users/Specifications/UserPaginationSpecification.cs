using AC.Domain.Modules.Users;
using AC.Domain.Specifications;
using Ardalis.Specification;

namespace AC.Application.Modules.Users.Specifications;

public sealed class UserPaginationSpecification : PaginationSpecification<User>
{
    public UserPaginationSpecification(int page, int perPage, Guid? roleId = null, Guid? supplierId = null)
        : base(page, perPage)
    {
        Query
            .Include(u => u.Role)
            .Include(u => u.Supplier)
            .OrderBy(u => u.FullName);

        if (roleId is not null)
            Query.Where(u => u.RoleId == roleId);

        if (supplierId is not null)
            Query.Where(u => u.SupplierId == supplierId);
    }
}