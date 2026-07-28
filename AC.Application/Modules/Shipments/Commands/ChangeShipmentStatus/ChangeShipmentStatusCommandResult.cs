using AC.Application.Abstractions.Messaging.Commands;
using AC.Domain.Modules.Shipments;

namespace AC.Application.Modules.Shipments.Commands.ChangeShipmentStatus;

public class ChangeShipmentStatusCommandResult : ICommandResult
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public ShipmentStatus Status { get; set; }
    public ShipmentObservation? Observation { get; set; }
    public string? DeliveryComment { get; set; }
}
