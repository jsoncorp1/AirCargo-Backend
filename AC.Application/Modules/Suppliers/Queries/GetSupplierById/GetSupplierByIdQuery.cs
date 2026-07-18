using AC.Application.Abstractions.Messaging.Queries;

namespace AC.Application.Modules.Suppliers.Queries.GetSupplierById;

public class GetSupplierByIdQuery : IQuery<GetSupplierByIdQueryResult>
{
    public Guid Id { get; set; }
}
