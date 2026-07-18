using AC.Application.Abstractions.Messaging.Commands;
using AC.Application.Modules.Suppliers.Specifications;
using AC.Domain.Modules.Suppliers;
using AC.Domain.Persistence;
using AC.Domain.Results;

namespace AC.Application.Modules.Suppliers.Commands.UpdateSupplier;

public class UpdateSupplierCommandHandler(
    IRepository<Supplier> repository,
    ICoreUnitOfWork unitOfWork)
    : ICommandHandler<UpdateSupplierCommand, UpdateSupplierCommandResult>
{
    public async Task<Result<UpdateSupplierCommandResult>> HandleAsync(
        UpdateSupplierCommand command, CancellationToken cancellationToken)
    {
        var supplier = await repository.GetBySpecificationAsync(
            new SupplierByIdSpecification(command.Id), cancellationToken);

        if (supplier is null)
            return Result.Fail<UpdateSupplierCommandResult>("Proveedor no encontrado.", "supplier.notfound");

        var duplicate = await repository.GetBySpecificationAsync(
            new SupplierByNameSpecification(command.Name), cancellationToken);

        if (duplicate is not null && duplicate.Id != supplier.Id)
            return Result.Fail<UpdateSupplierCommandResult>(
                "Ya existe otro proveedor con ese nombre.", "supplier.name.duplicate");

        supplier.Name = command.Name;
        supplier.Description = command.Description;

        await repository.UpdateAsync(supplier, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(new UpdateSupplierCommandResult
        {
            Id = supplier.Id,
            Name = supplier.Name
        });
    }
}
