namespace AC.Domain.Modules.Shipments;

public enum ShipmentStatus
{
    Pending,      // Pendiente
    InTransit,    // En tránsito (estado inicial al atender la orden)
    Observed,     // Observado (se setea automáticamente al registrar una observación)
    Delivered,    // Entregado
    Rejected,     // Rechazado (se setea cuando la observación es CustomerRefused)
    Returned      // Devuelto
}
