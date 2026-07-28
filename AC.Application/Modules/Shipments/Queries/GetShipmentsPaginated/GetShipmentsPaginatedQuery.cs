using AC.Application.Abstractions.Messaging.Queries;
using AC.Domain.Modules.Shipments;

namespace AC.Application.Modules.Shipments.Queries.GetShipmentsPaginated;

public class GetShipmentsPaginatedQuery : IQuery<GetShipmentsPaginatedQueryResult>
{
    public int Page { get; set; } = 1;
    public int PerPage { get; set; } = 10;

    // Usuario autenticado que consulta; define el alcance según su rol.
    public Guid UserId { get; set; }

    // Filtros opcionales y combinables; sin filtros se lista todo en general.
    public Guid? SupplierId { get; set; }
    public Guid? OriginBranchOfficeId { get; set; }
    public Guid? DestinationBranchOfficeId { get; set; }
    public ShipmentStatus? Status { get; set; }
}
