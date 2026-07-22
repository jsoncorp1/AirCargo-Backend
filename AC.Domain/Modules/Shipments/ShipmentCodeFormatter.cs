using AC.Domain.Modules.OrderDeliveries;

namespace AC.Domain.Modules.Shipments;

public static class ShipmentCodeFormatter
{
    public static string Format(OrderType orderType, DeliveryType deliveryType, int sequenceNumber)
    {
        var typePrefix = orderType switch
        {
            OrderType.Corporate => "COR",
            OrderType.Sporadic => "ESP",
            _ => throw new ArgumentOutOfRangeException(nameof(orderType))
        };

        var paymentPrefix = deliveryType switch
        {
            DeliveryType.Prepaid => "PAG",
            DeliveryType.CashOnDelivery => "CXC",
            _ => throw new ArgumentOutOfRangeException(nameof(deliveryType))
        };

        return $"{typePrefix}-{paymentPrefix}-{sequenceNumber:D6}";
    }
}
