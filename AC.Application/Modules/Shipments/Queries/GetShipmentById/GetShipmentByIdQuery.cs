using AC.Application.Abstractions.Messaging.Queries;

namespace AC.Application.Modules.Shipments.Queries.GetShipmentById;

public class GetShipmentByIdQuery : IQuery<GetShipmentByIdQueryResult>
{
    public Guid Id { get; set; }

    // Usuario autenticado que consulta; define el alcance según su rol.
    public Guid UserId { get; set; }
}
