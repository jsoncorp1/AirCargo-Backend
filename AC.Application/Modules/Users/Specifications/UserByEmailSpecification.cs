using AC.Domain.Modules.Users;
using Ardalis.Specification;

namespace AC.Application.Modules.Users.Specifications;

public sealed class UserByEmailSpecification : Specification<User>
{
    public UserByEmailSpecification(string email)
    {
        string normalizedEmail = email.Trim().ToLowerInvariant();

        Query
            .Where(u => u.Email.ToLower() == normalizedEmail)
            .Include(u => u.Role)
            .Include(u => u.Supplier);
    }
}