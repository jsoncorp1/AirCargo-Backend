using AC.Application.Abstractions.Messaging.Commands;

namespace AC.Application.Modules.Shipments.Commands.DeleteShipment;

public class DeleteShipmentCommandResult : ICommandResult
{
    public Guid Id { get; set; }
}
