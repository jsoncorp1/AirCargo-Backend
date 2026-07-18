using AC.Application.Abstractions.Messaging.Queries;
using AC.Application.Modules.Suppliers.Specifications;
using AC.Domain.Modules.Suppliers;
using AC.Domain.Persistence;
using AC.Domain.Results;

namespace AC.Application.Modules.Suppliers.Queries.GetSupplierById;

public class GetSupplierByIdQueryHandler(IRepository<Supplier> repository)
    : IQueryHandler<GetSupplierByIdQuery, GetSupplierByIdQueryResult>
{
    public async Task<Result<GetSupplierByIdQueryResult>> HandleAsync(
        GetSupplierByIdQuery query, CancellationToken cancellationToken)
    {
        var supplier = await repository.GetBySpecificationAsync(
            new SupplierByIdSpecification(query.Id), cancellationToken);

        if (supplier is null)
            return Result.Fail<GetSupplierByIdQueryResult>("Proveedor no encontrado.", "supplier.notfound");

        return Result.Success(new GetSupplierByIdQueryResult
        {
            Id = supplier.Id,
            Name = supplier.Name,
            Description = supplier.Description
        });
    }
}
