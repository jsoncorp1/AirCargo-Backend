using AC.Application.Abstractions.Messaging.Commands;
using AC.Application.Modules.Suppliers.Specifications;
using AC.Domain.Modules.Suppliers;
using AC.Domain.Persistence;
using AC.Domain.Results;

namespace AC.Application.Modules.Suppliers.Commands.CreateSupplier;

public class CreateSupplierCommandHandler(
    IRepository<Supplier> repository,
    ICoreUnitOfWork unitOfWork)
    : ICommandHandler<CreateSupplierCommand, CreateSupplierCommandResult>
{
    public async Task<Result<CreateSupplierCommandResult>> HandleAsync(
        CreateSupplierCommand command, CancellationToken cancellationToken)
    {
        Result validation = await ValidateAsync(command, cancellationToken);
        if (validation.Failure)
            return Result.Fail<CreateSupplierCommandResult>(validation.Error, validation.ErrorKey);

        var supplier = new Supplier
        {
            Id = Guid.NewGuid(),
            Name = command.Name,
            Description = command.Description,
            Department = command.Department
        };

        await repository.SaveAsync(supplier, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(new CreateSupplierCommandResult
        {
            Id = supplier.Id,
            Name = supplier.Name,
            Department = supplier.Department
        });
    }

    private async Task<Result> ValidateAsync(
        CreateSupplierCommand command, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(command.Name))
            return Result.Fail("El nombre es obligatorio.", "supplier.name.required");

        var existing = await repository.GetBySpecificationAsync(
            new SupplierByNameSpecification(command.Name), cancellationToken);

        if (existing is not null)
            return Result.Fail("Ya existe un proveedor con ese nombre.", "supplier.name.duplicate");

        return Result.Success();
    }
}
