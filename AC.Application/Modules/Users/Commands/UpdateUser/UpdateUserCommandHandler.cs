using AC.Application.Abstractions.Messaging.Commands;
using AC.Application.Modules.Roles.Specifications;
using AC.Application.Modules.Users.Specifications;
using AC.Domain.Modules.Roles;
using AC.Domain.Modules.Users;
using AC.Domain.Persistence;
using AC.Domain.Results;

namespace AC.Application.Modules.Users.Commands.UpdateUser;

public class UpdateUserCommandHandler(
    IRepository<User> userRepository,
    IRepository<Role> roleRepository,
    ICoreUnitOfWork unitOfWork)
    : ICommandHandler<UpdateUserCommand, UpdateUserCommandResult>
{
    public async Task<Result<UpdateUserCommandResult>> HandleAsync(
        UpdateUserCommand command, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetBySpecificationAsync(
            new UserByIdSpecification(command.Id), cancellationToken);

        if (user is null)
            return Result.Fail<UpdateUserCommandResult>("Usuario no encontrado.", "user.notfound");

        var duplicate = await userRepository.GetBySpecificationAsync(
            new UserByEmailSpecification(command.Email), cancellationToken);

        if (duplicate is not null && duplicate.Id != user.Id)
            return Result.Fail<UpdateUserCommandResult>(
                "Ya existe otro usuario con ese email.", "user.email.duplicate");

        var role = await roleRepository.GetBySpecificationAsync(
            new RoleByIdSpecification(command.RoleId), cancellationToken);

        if (role is null)
            return Result.Fail<UpdateUserCommandResult>("El rol indicado no existe.", "user.role.notfound");

        user.FullName = command.FullName;
        user.Email = command.Email;
        user.PhoneNumber = command.PhoneNumber;
        user.Dni = command.Dni;
        user.RoleId = command.RoleId;

        await userRepository.UpdateAsync(user, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(new UpdateUserCommandResult
        {
            Id = user.Id,
            FullName = user.FullName,
            Email = user.Email,
            PhoneNumber = user.PhoneNumber,
            Dni = user.Dni,
            RoleId = user.RoleId
        });
    }
}