using AC.Application.Abstractions.Messaging.Commands;
using AC.Application.Modules.Suppliers.Specifications;
using AC.Domain.Modules.Suppliers;
using AC.Domain.Persistence;
using AC.Domain.Results;

namespace AC.Application.Modules.Suppliers.Commands.DeleteSupplier;

public class DeleteSupplierCommandHandler(
    IRepository<Supplier> repository,
    ICoreUnitOfWork unitOfWork)
    : ICommandHandler<DeleteSupplierCommand, DeleteSupplierCommandResult>
{
    public async Task<Result<DeleteSupplierCommandResult>> HandleAsync(
        DeleteSupplierCommand command, CancellationToken cancellationToken)
    {
        var supplier = await repository.GetBySpecificationAsync(
            new SupplierByIdSpecification(command.Id), cancellationToken);

        if (supplier is null)
            return Result.Fail<DeleteSupplierCommandResult>("Proveedor no encontrado.", "supplier.notfound");

        supplier.Active = false; // soft-delete; el interceptor pone DeletedAt/DeletedBy

        await repository.UpdateAsync(supplier, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(new DeleteSupplierCommandResult { Id = supplier.Id });
    }
}
