using AC.Application.Abstractions.Messaging.Queries;

namespace AC.Application.Modules.Shipments.Queries.GetShipmentsPaginated;

public class GetShipmentsPaginatedQuery : IQuery<GetShipmentsPaginatedQueryResult>
{
    public int Page { get; set; } = 1;
    public int PerPage { get; set; } = 10;
    public Guid? SupplierId { get; set; }
}
