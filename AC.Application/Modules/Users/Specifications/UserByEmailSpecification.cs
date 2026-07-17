using AC.Domain.Modules.Users;
using Ardalis.Specification;

namespace AC.Application.Modules.Users.Specifications;

public sealed class UserByEmailSpecification : Specification<User>
{
    public UserByEmailSpecification(string email)
    {
        Query
            .Where(u => u.Email == email)
            .Include(u => u.Role);
    }
}