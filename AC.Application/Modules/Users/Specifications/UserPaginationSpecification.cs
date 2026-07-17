using AC.Domain.Modules.Users;
using AC.Domain.Specifications;
using Ardalis.Specification;

namespace AC.Application.Modules.Users.Specifications;

public sealed class UserPaginationSpecification : PaginationSpecification<User>
{
    public UserPaginationSpecification(int page, int perPage) : base(page, perPage)
    {
        Query
            .Include(u => u.Role)
            .OrderBy(u => u.FullName);
    }
}