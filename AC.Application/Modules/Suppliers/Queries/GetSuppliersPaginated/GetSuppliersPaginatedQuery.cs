using AC.Application.Abstractions.Messaging.Queries;
using AC.Domain.Modules.OrderDeliveries;

namespace AC.Application.Modules.Suppliers.Queries.GetSuppliersPaginated;

public class GetSuppliersPaginatedQuery : IQuery<GetSuppliersPaginatedQueryResult>
{
    public int Page { get; set; } = 1;
    public int PerPage { get; set; } = 10;
    public BolivianDepartment? Department { get; set; }
}
