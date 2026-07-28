using AC.Application.Abstractions.Messaging.Queries;

namespace AC.Application.Modules.OrderDeliveries.Queries.GetOrderDeliveryById;

public class GetOrderDeliveryByIdQuery : IQuery<GetOrderDeliveryByIdQueryResult>
{
    public Guid Id { get; set; }

    // Usuario autenticado que consulta; define el alcance según su rol.
    public Guid UserId { get; set; }
}
