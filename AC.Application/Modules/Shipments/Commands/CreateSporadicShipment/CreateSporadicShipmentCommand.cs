using AC.Application.Abstractions.Messaging.Commands;
using AC.Domain.Modules.OrderDeliveries;

namespace AC.Application.Modules.Shipments.Commands.CreateSporadicShipment;

public class CreateSporadicShipmentCommand : ICommand<CreateSporadicShipmentCommandResult>
{
    // Usuario autenticado que registra el envío; su sucursal es el origen
    // (también el departamento de origen de la orden).
    public Guid UserId { get; set; }
    public Guid DestinationBranchOfficeId { get; set; }

    public string SenderFullName { get; set; } = string.Empty;
    public string SenderPhone { get; set; } = string.Empty;
    public string SenderAddress { get; set; } = string.Empty;

    public BolivianDepartment DestinationDepartment { get; set; }
    public string ClientPhone { get; set; } = string.Empty;
    public string ClientFullName { get; set; } = string.Empty;
    public string ClientAddress { get; set; } = string.Empty;
    public DeliveryType DeliveryType { get; set; }
    public bool IsExpress { get; set; }

    public int PackageCount { get; set; }
    public string PackageDescription { get; set; } = string.Empty;

    public List<CreateSporadicShipmentLine> Lines { get; set; } = [];
}

public class CreateSporadicShipmentLine
{
    public string ArticleName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal Weight { get; set; }
    public decimal ShippingCost { get; set; }
}
