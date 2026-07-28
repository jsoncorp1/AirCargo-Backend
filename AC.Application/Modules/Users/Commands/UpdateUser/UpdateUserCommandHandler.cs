using AC.Application.Abstractions.Messaging.Commands;
using AC.Application.Modules.BranchOffices.Specifications;
using AC.Application.Modules.Roles.Specifications;
using AC.Application.Modules.Suppliers.Specifications;
using AC.Application.Modules.Users.Specifications;
using AC.Domain.Modules.BranchOffices;
using AC.Domain.Modules.Roles;
using AC.Domain.Modules.Suppliers;
using AC.Domain.Modules.Users;
using AC.Domain.Persistence;
using AC.Domain.Results;

namespace AC.Application.Modules.Users.Commands.UpdateUser;

public class UpdateUserCommandHandler(
    IRepository<User> userRepository,
    IRepository<Role> roleRepository,
    IRepository<Supplier> supplierRepository,
    IRepository<BranchOffice> branchOfficeRepository,
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

        // Coherencia rol ↔ alcance: usuarioempresa pertenece a un proveedor,
        // admin/conductor pertenecen a una sucursal.
        if (role.Name == RoleNames.UsuarioEmpresa)
        {
            if (command.SupplierId is null)
                return Result.Fail<UpdateUserCommandResult>(
                    "Un usuarioempresa debe tener proveedor.", "user.supplierid.required");

            if (command.BranchOfficeId is not null)
                return Result.Fail<UpdateUserCommandResult>(
                    "Un usuarioempresa no puede tener sucursal.", "user.branchofficeid.notallowed");
        }
        else if (role.Name is RoleNames.Admin or RoleNames.Conductor)
        {
            if (command.BranchOfficeId is null)
                return Result.Fail<UpdateUserCommandResult>(
                    $"Un {role.Name} debe tener sucursal.", "user.branchofficeid.required");

            if (command.SupplierId is not null)
                return Result.Fail<UpdateUserCommandResult>(
                    $"Un {role.Name} no puede tener proveedor.", "user.supplierid.notallowed");
        }

        var actor = await userRepository.GetBySpecificationAsync(
            new UserByIdSpecification(command.ActorUserId), cancellationToken);

        if (actor is null)
            return Result.Fail<UpdateUserCommandResult>(
                "El usuario autenticado no existe.", "user.actor.notfound");

        // El admin solo gestiona conductores de su propia sucursal, y no puede
        // sacarlos de ella ni cambiarles el rol.
        if (actor.Role.Name == RoleNames.Admin)
        {
            if (user.Role.Name != RoleNames.Conductor || user.BranchOfficeId != actor.BranchOfficeId)
                return Result.Fail<UpdateUserCommandResult>(
                    "Un admin solo puede editar conductores de su sucursal.", "user.access.forbidden");

            if (role.Name != RoleNames.Conductor)
                return Result.Fail<UpdateUserCommandResult>(
                    "Un admin no puede cambiar el rol de un conductor.", "user.role.forbidden");

            if (command.BranchOfficeId != actor.BranchOfficeId)
                return Result.Fail<UpdateUserCommandResult>(
                    "Un admin no puede mover conductores a otra sucursal.", "user.branchoffice.forbidden");
        }

        Supplier? newSupplier = null;

        if (command.SupplierId is not null)
        {
            newSupplier = await supplierRepository.GetBySpecificationAsync(
                new SupplierByIdSpecification(command.SupplierId.Value), cancellationToken);

            if (newSupplier is null)
                return Result.Fail<UpdateUserCommandResult>(
                    "El proveedor indicado no existe.", "user.supplier.notfound");
        }

        if (command.BranchOfficeId is not null)
        {
            var branchOffice = await branchOfficeRepository.GetBySpecificationAsync(
                new BranchOfficeByIdSpecification(command.BranchOfficeId.Value), cancellationToken);

            if (branchOffice is null)
                return Result.Fail<UpdateUserCommandResult>(
                    "La sucursal indicada no existe.", "user.branchoffice.notfound");
        }

        var previousSupplier = user.Supplier;

        user.FullName = command.FullName;
        user.Email = command.Email.Trim().ToLowerInvariant();
        user.PhoneNumber = command.PhoneNumber;
        user.Dni = command.Dni;
        user.RoleId = command.RoleId;
        user.SupplierId = command.SupplierId;
        user.Supplier = newSupplier;
        user.BranchOfficeId = command.BranchOfficeId;

        await userRepository.UpdateAsync(user, cancellationToken);

        if (previousSupplier?.Id != newSupplier?.Id)
        {
            if (previousSupplier is not null)
            {
                previousSupplier.UserQuantity = Math.Max(0, previousSupplier.UserQuantity - 1);
                await supplierRepository.UpdateAsync(previousSupplier, cancellationToken);
            }

            if (newSupplier is not null)
            {
                newSupplier.UserQuantity += 1;
                await supplierRepository.UpdateAsync(newSupplier, cancellationToken);
            }
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(new UpdateUserCommandResult
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
}