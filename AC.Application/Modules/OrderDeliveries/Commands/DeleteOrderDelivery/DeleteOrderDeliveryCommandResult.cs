using AC.Application.Abstractions.Messaging.Commands;

namespace AC.Application.Modules.OrderDeliveries.Commands.DeleteOrderDelivery;

public class DeleteOrderDeliveryCommandResult : ICommandResult
{
    public Guid Id { get; set; }
}
