using AC.Domain.Modules.Users;
using Ardalis.Specification;

namespace AC.Application.Modules.Users.Specifications;

public sealed class UserByIdSpecification : Specification<User>
{
    public UserByIdSpecification(Guid id)
    {
        Query
            .Where(u => u.Id == id)
            .Include(u => u.Role)
            .Include(u => u.Supplier);
    }
}