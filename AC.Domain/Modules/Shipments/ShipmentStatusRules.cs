namespace AC.Domain.Modules.Shipments;

// Reglas de transición de estados del envío. El estado también puede cambiar
// indirectamente al registrar una observación: CustomerRefused → Rejected,
// cualquier otra observación → Observed.
public static class ShipmentStatusRules
{
    private static readonly Dictionary<ShipmentStatus, ShipmentStatus[]> Transitions = new()
    {
        [ShipmentStatus.Pending] = [ShipmentStatus.InTransit],
        [ShipmentStatus.InTransit] = [ShipmentStatus.Observed, ShipmentStatus.Delivered, ShipmentStatus.Rejected, ShipmentStatus.Returned],
        [ShipmentStatus.Observed] = [ShipmentStatus.InTransit, ShipmentStatus.Delivered, ShipmentStatus.Rejected, ShipmentStatus.Returned],
        [ShipmentStatus.Rejected] = [ShipmentStatus.Returned],
        [ShipmentStatus.Delivered] = [],
        [ShipmentStatus.Returned] = []
    };

    public static bool CanTransition(ShipmentStatus from, ShipmentStatus to) =>
        from != to && Transitions.TryGetValue(from, out var allowed) && allowed.Contains(to);

    // Solo tiene sentido observar un envío que está en la calle.
    public static bool CanReceiveObservation(ShipmentStatus status) =>
        status is ShipmentStatus.InTransit or ShipmentStatus.Observed;

    public static ShipmentStatus StatusFor(ShipmentObservation observation) =>
        observation == ShipmentObservation.CustomerRefused
            ? ShipmentStatus.Rejected
            : ShipmentStatus.Observed;
}
