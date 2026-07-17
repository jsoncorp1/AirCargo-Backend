using AC.Domain.Modules.Roles;
using Ardalis.Specification;

namespace AC.Application.Modules.Roles.Specifications;

public class RoleByIdSpecification : Specification<Role>
{
    public RoleByIdSpecification(Guid id) => Query.Where(r => r.Id == id);
}