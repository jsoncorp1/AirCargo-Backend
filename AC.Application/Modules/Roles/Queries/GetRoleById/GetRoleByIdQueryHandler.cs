using AC.Application.Abstractions.Messaging.Queries;
using AC.Application.Modules.Roles.Specifications;
using AC.Domain.Modules.Roles;
using AC.Domain.Persistence;
using AC.Domain.Results;

namespace AC.Application.Modules.Roles.Queries.GetRoleById;

public class GetRoleByIdQueryHandler(IRepository<Role> repository)
    : IQueryHandler<GetRoleByIdQuery, GetRoleByIdQueryResult>
{
    public async Task<Result<GetRoleByIdQueryResult>> HandleAsync(
        GetRoleByIdQuery query, CancellationToken cancellationToken)
    {
        var role = await repository.GetBySpecificationAsync(
            new RoleByIdSpecification(query.Id), cancellationToken);

        if (role is null)
            return Result.Fail<GetRoleByIdQueryResult>("Rol no encontrado.", "role.notfound");

        return Result.Success(new GetRoleByIdQueryResult
        {
            Id = role.Id,
            Name = role.Name,
            Description = role.Description
        });
    }
}