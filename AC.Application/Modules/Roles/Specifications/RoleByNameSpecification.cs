using AC.Domain.Modules.Roles;
using Ardalis.Specification;

namespace AC.Application.Modules.Roles.Specifications;

public class RoleByNameSpecification : Specification<Role>
{
    public RoleByNameSpecification(string name) => Query.Where(r => r.Name == name);
}