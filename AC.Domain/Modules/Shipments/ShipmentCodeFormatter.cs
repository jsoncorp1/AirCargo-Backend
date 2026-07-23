using AC.Domain.Modules.OrderDeliveries;

namespace AC.Domain.Modules.Shipments;

public static class ShipmentCodeFormatter
{
    public static string Format(OrderType orderType, int sequenceNumber)
    {
        var typePrefix = orderType switch
        {
            OrderType.Corporate => "COR",
            OrderType.Sporadic => "ESP",
            _ => throw new ArgumentOutOfRangeException(nameof(orderType))
        };

        return $"{typePrefix}-{sequenceNumber:D6}";
    }
}
