using AC.Application.Abstractions.Messaging.Commands;
using AC.Domain.Modules.Shipments;

namespace AC.Application.Modules.Shipments.Commands.ChangeShipmentStatus;

// Cambia el estado del envío, registra una observación del delivery o su
// comentario libre. Si viene Observation, el estado se deriva de ella
// (CustomerRefused → Rejected, resto → Observed) y no se acepta Status explícito.
public class ChangeShipmentStatusCommand : ICommand<ChangeShipmentStatusCommandResult>
{
    public Guid Id { get; set; }

    // Usuario autenticado; admin/conductor solo pueden tocar envíos de su sucursal.
    public Guid UserId { get; set; }

    public ShipmentStatus? Status { get; set; }
    public ShipmentObservation? Observation { get; set; }
    public string? DeliveryComment { get; set; }
}
