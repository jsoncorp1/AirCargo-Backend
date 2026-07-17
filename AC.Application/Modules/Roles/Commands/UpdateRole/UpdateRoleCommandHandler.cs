using AC.Application.Abstractions.Messaging.Commands;
using AC.Application.Modules.Roles.Specifications;
using AC.Domain.Modules.Roles;
using AC.Domain.Persistence;
using AC.Domain.Results;

namespace AC.Application.Modules.Roles.Commands.UpdateRole;

public class UpdateRoleCommandHandler(
    IRepository<Role> repository,
    ICoreUnitOfWork unitOfWork)
    : ICommandHandler<UpdateRoleCommand, UpdateRoleCommandResult>
{
    public async Task<Result<UpdateRoleCommandResult>> HandleAsync(
        UpdateRoleCommand command, CancellationToken cancellationToken)
    {
        var role = await repository.GetBySpecificationAsync(
            new RoleByIdSpecification(command.Id), cancellationToken);

        if (role is null)
            return Result.Fail<UpdateRoleCommandResult>("Rol no encontrado.", "role.notfound");

        var duplicate = await repository.GetBySpecificationAsync(
            new RoleByNameSpecification(command.Name), cancellationToken);

        if (duplicate is not null && duplicate.Id != role.Id)
            return Result.Fail<UpdateRoleCommandResult>(
                "Ya existe otro rol con ese nombre.", "role.name.duplicate");

        role.Name = command.Name;
        role.Description = command.Description;

        await repository.UpdateAsync(role, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(new UpdateRoleCommandResult
        {
            Id = role.Id,
            Name = role.Name
        });
    }
}