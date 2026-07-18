using AC.Application.Abstractions.Messaging.Queries;
using AC.Application.Modules.Shipments.Specifications;
using AC.Domain.Modules.Shipments;
using AC.Domain.Persistence;
using AC.Domain.Results;

namespace AC.Application.Modules.Shipments.Queries.GetShipmentById;

public class GetShipmentByIdQueryHandler(IRepository<Shipment> repository)
    : IQueryHandler<GetShipmentByIdQuery, GetShipmentByIdQueryResult>
{
    public async Task<Result<GetShipmentByIdQueryResult>> HandleAsync(
        GetShipmentByIdQuery query, CancellationToken cancellationToken)
    {
        var shipment = await repository.GetBySpecificationAsync(
            new ShipmentByIdSpecification(query.Id), cancellationToken);

        if (shipment is null)
            return Result.Fail<GetShipmentByIdQueryResult>(
                "Envío no encontrado.", "shipment.notfound");

        return Result.Success(new GetShipmentByIdQueryResult
        {
            Id = shipment.Id,
            OrderDeliveryId = shipment.OrderDeliveryId,
            NumeroGuia = shipment.Correlativo.ToString("D8"),
            ClienteNombreCompleto = shipment.OrderDelivery.ClienteNombreCompleto,
            ClienteDireccion = shipment.OrderDelivery.ClienteDireccion,
            Departamento = shipment.OrderDelivery.Departamento.ToString(),
            TotalWeight = shipment.TotalWeight,
            ShippingPrice = shipment.ShippingPrice,
            CreatedAt = shipment.CreatedAt,
            Details = shipment.ShipmentDetails.Select(d => new ShipmentDetailItem
            {
                Id = d.Id,
                OrderDeliveryDetailId = d.OrderDeliveryDetailId,
                ArticleName = d.OrderDeliveryDetail.ArticleName,
                Quantity = d.OrderDeliveryDetail.Quantity,
                UnitPrice = d.OrderDeliveryDetail.UnitPrice,
                Weight = d.Weight,
                ShippingCost = d.ShippingCost
            }).ToList()
        });
    }
}
