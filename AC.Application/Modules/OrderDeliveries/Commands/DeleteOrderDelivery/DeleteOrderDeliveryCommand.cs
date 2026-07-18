using AC.Application.Abstractions.Messaging.Commands;

namespace AC.Application.Modules.OrderDeliveries.Commands.DeleteOrderDelivery;

public class DeleteOrderDeliveryCommand : ICommand<DeleteOrderDeliveryCommandResult>
{
    public Guid Id { get; set; }
}
