using AC.Application.Abstractions.Messaging.Commands;
using AC.Application.Modules.Roles.Specifications;
using AC.Domain.Modules.Roles;
using AC.Domain.Persistence;
using AC.Domain.Results;

namespace AC.Application.Modules.Roles.Commands.CreateRole;

public class CreateRoleCommandHandler(
    IRepository<Role> repository,
    ICoreUnitOfWork unitOfWork)
    : ICommandHandler<CreateRoleCommand, CreateRoleCommandResult>
{
    public async Task<Result<CreateRoleCommandResult>> HandleAsync(
        CreateRoleCommand command, CancellationToken cancellationToken)
    {
        Result validation = await ValidateAsync(command, cancellationToken);
        if (validation.Failure)
            return Result.Fail<CreateRoleCommandResult>(validation.Error, validation.ErrorKey);

        var role = new Role
        {
            Id = Guid.NewGuid(),
            Name = command.Name,
            Description = command.Description
            // Active / CreatedAt / CreatedBy los pone el AuditInterceptor
        };

        await repository.SaveAsync(role, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(new CreateRoleCommandResult
        {
            Id = role.Id,
            Name = role.Name
        });
    }

    private async Task<Result> ValidateAsync(
        CreateRoleCommand command, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(command.Name))
            return Result.Fail("El nombre es obligatorio.", "role.name.required");

        var existing = await repository.GetBySpecificationAsync(
            new RoleByNameSpecification(command.Name), cancellationToken);

        if (existing is not null)
            return Result.Fail("Ya existe un rol con ese nombre.", "role.name.duplicate");

        return Result.Success();
    }
}