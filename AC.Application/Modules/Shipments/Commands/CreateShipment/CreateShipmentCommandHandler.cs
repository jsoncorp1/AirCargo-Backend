using AC.Application.Abstractions.Messaging.Commands;
using AC.Application.Modules.OrderDeliveries.Specifications;
using AC.Domain.Modules.OrderDeliveries;
using AC.Domain.Modules.Shipments;
using AC.Domain.Persistence;
using AC.Domain.Results;

namespace AC.Application.Modules.Shipments.Commands.CreateShipment;

public class CreateShipmentCommandHandler(
    IRepository<OrderDelivery> orderDeliveryRepository,
    IRepository<Shipment> shipmentRepository,
    IRepository<ShipmentDetail> shipmentDetailRepository,
    ICoreUnitOfWork unitOfWork)
    : ICommandHandler<CreateShipmentCommand, CreateShipmentCommandResult>
{
    public async Task<Result<CreateShipmentCommandResult>> HandleAsync(
        CreateShipmentCommand command, CancellationToken cancellationToken)
    {
        var order = await orderDeliveryRepository.GetBySpecificationAsync(
            new OrderDeliveryByIdSpecification(command.OrderDeliveryId), cancellationToken);

        if (order is null)
            return Result.Fail<CreateShipmentCommandResult>(
                "Orden no encontrada.", "shipment.orderdelivery.notfound");

        if (order.Shipments.Any(s => s.Active))
            return Result.Fail<CreateShipmentCommandResult>(
                "La orden ya fue atendida.", "shipment.alreadyattended");

        if (command.Lines.Count == 0)
            return Result.Fail<CreateShipmentCommandResult>(
                "El envío debe tener al menos una línea.", "shipment.lines.required");

        if (command.PackageCount <= 0)
            return Result.Fail<CreateShipmentCommandResult>(
                "La cantidad de paquetes debe ser mayor a cero.", "shipment.packagecount.invalid");

        if (string.IsNullOrWhiteSpace(command.PackageDescription))
            return Result.Fail<CreateShipmentCommandResult>(
                "La descripción de los paquetes es obligatoria.", "shipment.packagedescription.required");

        var orderDetailIds = order.OrderDeliveryDetails.Select(d => d.Id).ToHashSet();
        var lineDetailIds = command.Lines.Select(l => l.OrderDeliveryDetailId).ToHashSet();

        if (lineDetailIds.Count != command.Lines.Count || !lineDetailIds.SetEquals(orderDetailIds))
            return Result.Fail<CreateShipmentCommandResult>(
                "Las líneas del envío deben coincidir exactamente con las líneas de la orden.",
                "shipment.lines.mismatch");

        foreach (var line in command.Lines)
        {
            if (line.Weight <= 0)
                return Result.Fail<CreateShipmentCommandResult>(
                    "El peso de cada línea debe ser mayor a cero.", "shipment.weight.invalid");

            if (line.ShippingCost < 0)
                return Result.Fail<CreateShipmentCommandResult>(
                    "El costo de envío de cada línea no puede ser negativo.", "shipment.shippingcost.invalid");
        }

        int sequenceNumber = (await shipmentRepository.MaxAsync(s => s.SequenceNumber, cancellationToken) ?? 0) + 1;
        string code = ShipmentCodeFormatter.Format(order.OrderType, sequenceNumber);

        var shipment = new Shipment
        {
            Id = Guid.NewGuid(),
            OrderDeliveryId = order.Id,
            SequenceNumber = sequenceNumber,
            Code = code,
            TotalWeight = command.Lines.Sum(l => l.Weight),
            ShippingPrice = command.Lines.Sum(l => l.ShippingCost),
            PackageCount = command.PackageCount,
            PackageDescription = command.PackageDescription
        };

        var details = command.Lines.Select(l => new ShipmentDetail
        {
            Id = Guid.NewGuid(),
            ShipmentId = shipment.Id,
            OrderDeliveryDetailId = l.OrderDeliveryDetailId,
            Weight = l.Weight,
            ShippingCost = l.ShippingCost
        }).ToList();

        await shipmentRepository.SaveAsync(shipment, cancellationToken);
        await shipmentDetailRepository.SaveAsync(details.ToArray(), cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(new CreateShipmentCommandResult
        {
            Id = shipment.Id,
            OrderDeliveryId = shipment.OrderDeliveryId,
            WaybillNumber = shipment.SequenceNumber.ToString("D8"),
            Code = shipment.Code,
            TotalWeight = shipment.TotalWeight,
            ShippingPrice = shipment.ShippingPrice,
            PackageCount = shipment.PackageCount,
            PackageDescription = shipment.PackageDescription,
            Details = details.Select(d => new CreateShipmentDetailResult
            {
                Id = d.Id,
                OrderDeliveryDetailId = d.OrderDeliveryDetailId,
                Weight = d.Weight,
                ShippingCost = d.ShippingCost
            }).ToList()
        });
    }
}
