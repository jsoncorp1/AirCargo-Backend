using AC.Application.Abstractions.Messaging.Commands;
using AC.Application.Modules.Roles.Specifications;
using AC.Domain.Modules.Roles;
using AC.Domain.Persistence;
using AC.Domain.Results;

namespace AC.Application.Modules.Roles.Commands.DeleteRole;

public class DeleteRoleCommandHandler(
    IRepository<Role> repository,
    ICoreUnitOfWork unitOfWork)
    : ICommandHandler<DeleteRoleCommand, DeleteRoleCommandResult>
{
    public async Task<Result<DeleteRoleCommandResult>> HandleAsync(
        DeleteRoleCommand command, CancellationToken cancellationToken)
    {
        var role = await repository.GetBySpecificationAsync(
            new RoleByIdSpecification(command.Id), cancellationToken);

        if (role is null)
            return Result.Fail<DeleteRoleCommandResult>("Rol no encontrado.", "role.notfound");

        role.Active = false; // soft-delete; el interceptor pone DeletedAt/DeletedBy

        await repository.UpdateAsync(role, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(new DeleteRoleCommandResult { Id = role.Id });
    }
}