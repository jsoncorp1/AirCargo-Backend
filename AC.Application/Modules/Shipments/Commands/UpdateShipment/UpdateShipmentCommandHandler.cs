using AC.Application.Abstractions.Messaging.Commands;
using AC.Application.Modules.Shipments.Specifications;
using AC.Domain.Modules.Shipments;
using AC.Domain.Persistence;
using AC.Domain.Results;

namespace AC.Application.Modules.Shipments.Commands.UpdateShipment;

public class UpdateShipmentCommandHandler(
    IRepository<Shipment> shipmentRepository,
    IRepository<ShipmentDetail> shipmentDetailRepository,
    ICoreUnitOfWork unitOfWork)
    : ICommandHandler<UpdateShipmentCommand, UpdateShipmentCommandResult>
{
    public async Task<Result<UpdateShipmentCommandResult>> HandleAsync(
        UpdateShipmentCommand command, CancellationToken cancellationToken)
    {
        var shipment = await shipmentRepository.GetBySpecificationAsync(
            new ShipmentByIdSpecification(command.Id), cancellationToken);

        if (shipment is null)
            return Result.Fail<UpdateShipmentCommandResult>(
                "Envío no encontrado.", "shipment.notfound");

        if (command.Lines.Count == 0)
            return Result.Fail<UpdateShipmentCommandResult>(
                "El envío debe tener al menos una línea.", "shipment.lines.required");

        if (command.PackageCount <= 0)
            return Result.Fail<UpdateShipmentCommandResult>(
                "La cantidad de paquetes debe ser mayor a cero.", "shipment.packagecount.invalid");

        if (string.IsNullOrWhiteSpace(command.PackageDescription))
            return Result.Fail<UpdateShipmentCommandResult>(
                "La descripción de los paquetes es obligatoria.", "shipment.packagedescription.required");

        var existingDetails = shipment.ShipmentDetails.ToDictionary(d => d.Id);
        var lineDetailIds = command.Lines.Select(l => l.ShipmentDetailId).ToHashSet();

        if (lineDetailIds.Count != command.Lines.Count || !lineDetailIds.SetEquals(existingDetails.Keys))
            return Result.Fail<UpdateShipmentCommandResult>(
                "Las líneas enviadas deben coincidir exactamente con las líneas del envío.",
                "shipment.lines.mismatch");

        foreach (var line in command.Lines)
        {
            if (line.Weight <= 0)
                return Result.Fail<UpdateShipmentCommandResult>(
                    "El peso de cada línea debe ser mayor a cero.", "shipment.weight.invalid");

            if (line.ShippingCost < 0)
                return Result.Fail<UpdateShipmentCommandResult>(
                    "El costo de envío de cada línea no puede ser negativo.", "shipment.shippingcost.invalid");
        }

        foreach (var line in command.Lines)
        {
            var detail = existingDetails[line.ShipmentDetailId];
            detail.Weight = line.Weight;
            detail.ShippingCost = line.ShippingCost;
        }

        shipment.TotalWeight = shipment.ShipmentDetails.Sum(d => d.Weight);
        shipment.ShippingPrice = shipment.ShipmentDetails.Sum(d => d.ShippingCost);
        shipment.PackageCount = command.PackageCount;
        shipment.PackageDescription = command.PackageDescription;

        await shipmentDetailRepository.UpdateAsync(shipment.ShipmentDetails.ToArray(), cancellationToken);
        await shipmentRepository.UpdateAsync(shipment, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(new UpdateShipmentCommandResult
        {
            Id = shipment.Id,
            OrderDeliveryId = shipment.OrderDeliveryId,
            WaybillNumber = shipment.SequenceNumber.ToString("D8"),
            Code = shipment.Code,
            TotalWeight = shipment.TotalWeight,
            ShippingPrice = shipment.ShippingPrice,
            PackageCount = shipment.PackageCount,
            PackageDescription = shipment.PackageDescription,
            Details = shipment.ShipmentDetails.Select(d => new UpdateShipmentDetailResult
            {
                Id = d.Id,
                OrderDeliveryDetailId = d.OrderDeliveryDetailId,
                Weight = d.Weight,
                ShippingCost = d.ShippingCost
            }).ToList()
        });
    }
}
