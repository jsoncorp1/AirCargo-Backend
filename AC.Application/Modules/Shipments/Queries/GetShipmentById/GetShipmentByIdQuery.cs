using AC.Application.Abstractions.Messaging.Queries;

namespace AC.Application.Modules.Shipments.Queries.GetShipmentById;

public class GetShipmentByIdQuery : IQuery<GetShipmentByIdQueryResult>
{
    public Guid Id { get; set; }
}
