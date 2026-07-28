using AC.Application.Abstractions.Messaging.Commands;

namespace AC.Application.Modules.Shipments.Commands.DeleteShipment;

public class DeleteShipmentCommand : ICommand<DeleteShipmentCommandResult>
{
    public Guid Id { get; set; }

    // Usuario autenticado que elimina; admin solo puede eliminar envíos de su sucursal.
    public Guid UserId { get; set; }
}
