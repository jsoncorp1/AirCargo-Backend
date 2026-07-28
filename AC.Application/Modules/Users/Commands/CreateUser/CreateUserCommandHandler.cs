using AC.Application.Abstractions.Messaging.Commands;
using AC.Application.Modules.BranchOffices.Specifications;
using AC.Application.Modules.Roles.Specifications;
using AC.Application.Modules.Suppliers.Specifications;
using AC.Application.Modules.Users.Specifications;
using AC.Application.Services.Security;
using AC.Domain.Modules.BranchOffices;
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
    IRepository<BranchOffice> branchOfficeRepository,
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

        Supplier? supplier = command.SupplierId is null
            ? null
            : await supplierRepository.GetBySpecificationAsync(
                new SupplierByIdSpecification(command.SupplierId.Value), cancellationToken);

        var user = new User
        {
            Id = Guid.NewGuid(),
            FullName = command.FullName,
            Email = command.Email.Trim().ToLowerInvariant(),
            PasswordHash = passwordHasher.Hash(command.Password),
            PhoneNumber = command.PhoneNumber,
            Dni = command.Dni,
            RoleId = command.RoleId,
            SupplierId = command.SupplierId,
            BranchOfficeId = command.BranchOfficeId
        };

        await userRepository.SaveAsync(user, cancellationToken);

        if (supplier is not null)
        {
            supplier.UserQuantity += 1;
            await supplierRepository.UpdateAsync(supplier, cancellationToken);
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(new CreateUserCommandResult
        {
            Id = user.Id,
            FullName = user.FullName,
            Email = user.Email,
            PhoneNumber = user.PhoneNumber,
            Dni = user.Dni,
            RoleId = user.RoleId,
            SupplierId = user.SupplierId,
            BranchOfficeId = user.BranchOfficeId
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

        // Coherencia rol ↔ alcance: usuarioempresa pertenece a un proveedor,
        // admin/conductor pertenecen a una sucursal.
        if (role.Name == RoleNames.UsuarioEmpresa)
        {
            if (command.SupplierId is null)
                return Result.Fail("Un usuarioempresa debe tener proveedor.", "user.supplierid.required");

            if (command.BranchOfficeId is not null)
                return Result.Fail("Un usuarioempresa no puede tener sucursal.", "user.branchofficeid.notallowed");
        }
        else if (role.Name is RoleNames.Admin or RoleNames.Conductor)
        {
            if (command.BranchOfficeId is null)
                return Result.Fail($"Un {role.Name} debe tener sucursal.", "user.branchofficeid.required");

            if (command.SupplierId is not null)
                return Result.Fail($"Un {role.Name} no puede tener proveedor.", "user.supplierid.notallowed");
        }

        var actor = await userRepository.GetBySpecificationAsync(
            new UserByIdSpecification(command.ActorUserId), cancellationToken);

        if (actor is null)
            return Result.Fail("El usuario autenticado no existe.", "user.actor.notfound");

        // El admin solo gestiona conductores de su propia sucursal.
        if (actor.Role.Name == RoleNames.Admin)
        {
            if (role.Name != RoleNames.Conductor)
                return Result.Fail(
                    "Un admin solo puede crear usuarios con rol conductor.", "user.role.forbidden");

            if (command.BranchOfficeId != actor.BranchOfficeId)
                return Result.Fail(
                    "Un admin solo puede crear conductores de su sucursal.", "user.branchoffice.forbidden");
        }

        if (command.SupplierId is not null)
        {
            var supplier = await supplierRepository.GetBySpecificationAsync(
                new SupplierByIdSpecification(command.SupplierId.Value), cancellationToken);

            if (supplier is null)
                return Result.Fail("El proveedor indicado no existe.", "user.supplier.notfound");
        }

        if (command.BranchOfficeId is not null)
        {
            var branchOffice = await branchOfficeRepository.GetBySpecificationAsync(
                new BranchOfficeByIdSpecification(command.BranchOfficeId.Value), cancellationToken);

            if (branchOffice is null)
                return Result.Fail("La sucursal indicada no existe.", "user.branchoffice.notfound");
        }

        return Result.Success();
    }
}