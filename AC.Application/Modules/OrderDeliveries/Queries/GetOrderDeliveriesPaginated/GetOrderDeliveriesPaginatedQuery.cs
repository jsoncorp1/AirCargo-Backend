using AC.Application.Abstractions.Messaging.Queries;

namespace AC.Application.Modules.OrderDeliveries.Queries.GetOrderDeliveriesPaginated;

public class GetOrderDeliveriesPaginatedQuery : IQuery<GetOrderDeliveriesPaginatedQueryResult>
{
    public int Page { get; set; } = 1;
    public int PerPage { get; set; } = 10;

    // Usuario autenticado que consulta; define el alcance según su rol.
    public Guid UserId { get; set; }

    public Guid? SupplierId { get; set; }
    public bool? Unattended { get; set; }
}
