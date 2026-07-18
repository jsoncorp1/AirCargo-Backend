using AC.Application.Abstractions.Messaging.Commands;
using AC.Application.Modules.Roles.Specifications;
using AC.Application.Modules.Suppliers.Specifications;
using AC.Application.Modules.Users.Specifications;
using AC.Application.Services.Security;
using AC.Domain.Modules.Roles;
using AC.Domain.Modules.Suppliers;
using AC.Domain.Modules.Users;
using AC.Domain.Persistence;
using AC.Domain.Results;

namespace AC.Application.Modules.Users.Commands.CreateUser;

public class CreateUserCommandHandler(
    IRepository<User> userRepository,
    IRepository<Role> roleRepository,
    IRepository<Supplier> supplierRepository,
    IPasswordHasher passwordHasher,
    ICoreUnitOfWork unitOfWork)
    : ICommandHandler<CreateUserCommand, CreateUserCommandResult>
{
    public async Task<Result<CreateUserCommandResult>> HandleAsync(
        CreateUserCommand command, CancellationToken cancellationToken)
    {
        Result validation = await ValidateAsync(command, cancellationToken);
        if (validation.Failure)
            return Result.Fail<CreateUserCommandResult>(validation.Error, validation.ErrorKey);

        var user = new User
        {
            Id = Guid.NewGuid(),
            FullName = command.FullName,
            Email = command.Email,
            PasswordHash = passwordHasher.Hash(command.Password),
            PhoneNumber = command.PhoneNumber,
            Dni = command.Dni,
            RoleId = command.RoleId,
            SupplierId = command.SupplierId
        };

        await userRepository.SaveAsync(user, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(new CreateUserCommandResult
        {
            Id = user.Id,
            FullName = user.FullName,
            Email = user.Email,
            PhoneNumber = user.PhoneNumber,
            Dni = user.Dni,
            RoleId = user.RoleId,
            SupplierId = user.SupplierId
        });
    }

    private async Task<Result> ValidateAsync(
        CreateUserCommand command, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(command.FullName))
            return Result.Fail("El nombre es obligatorio.", "user.fullname.required");

        if (string.IsNullOrWhiteSpace(command.Email))
            return Result.Fail("El email es obligatorio.", "user.email.required");

        if (string.IsNullOrWhiteSpace(command.Password))
            return Result.Fail("La contraseña es obligatoria.", "user.password.required");

        if (command.RoleId == Guid.Empty)
            return Result.Fail("El rol es obligatorio.", "user.roleid.required");

        var existingEmail = await userRepository.GetBySpecificationAsync(
            new UserByEmailSpecification(command.Email), cancellationToken);

        if (existingEmail is not null)
            return Result.Fail("Ya existe un usuario con ese email.", "user.email.duplicate");

        var role = await roleRepository.GetBySpecificationAsync(
            new RoleByIdSpecification(command.RoleId), cancellationToken);

        if (role is null)
            return Result.Fail("El rol indicado no existe.", "user.role.notfound");

        if (command.SupplierId is not null)
        {
            var supplier = await supplierRepository.GetBySpecificationAsync(
                new SupplierByIdSpecification(command.SupplierId.Value), cancellationToken);

            if (supplier is null)
                return Result.Fail("El proveedor indicado no existe.", "user.supplier.notfound");
        }

        return Result.Success();
    }
}