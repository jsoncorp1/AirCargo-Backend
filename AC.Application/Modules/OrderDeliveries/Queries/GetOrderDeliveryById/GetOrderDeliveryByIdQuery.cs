using AC.Application.Abstractions.Messaging.Queries;

namespace AC.Application.Modules.OrderDeliveries.Queries.GetOrderDeliveryById;

public class GetOrderDeliveryByIdQuery : IQuery<GetOrderDeliveryByIdQueryResult>
{
    public Guid Id { get; set; }
}
