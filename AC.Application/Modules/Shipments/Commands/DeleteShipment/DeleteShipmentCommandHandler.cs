using AC.Application.Abstractions.Messaging.Commands;
using AC.Application.Modules.Shipments.Specifications;
using AC.Domain.Modules.Shipments;
using AC.Domain.Persistence;
using AC.Domain.Results;

namespace AC.Application.Modules.Shipments.Commands.DeleteShipment;

public class DeleteShipmentCommandHandler(
    IRepository<Shipment> shipmentRepository,
    IRepository<ShipmentDetail> shipmentDetailRepository,
    ICoreUnitOfWork unitOfWork)
    : ICommandHandler<DeleteShipmentCommand, DeleteShipmentCommandResult>
{
    public async Task<Result<DeleteShipmentCommandResult>> HandleAsync(
        DeleteShipmentCommand command, CancellationToken cancellationToken)
    {
        var shipment = await shipmentRepository.GetBySpecificationAsync(
            new ShipmentByIdSpecification(command.Id), cancellationToken);

        if (shipment is null)
            return Result.Fail<DeleteShipmentCommandResult>(
                "Envío no encontrado.", "shipment.notfound");

        shipment.Active = false; // soft-delete; el interceptor pone DeletedAt/DeletedBy

        foreach (var detail in shipment.ShipmentDetails)
            detail.Active = false;

        await shipmentRepository.UpdateAsync(shipment, cancellationToken);

        if (shipment.ShipmentDetails.Count > 0)
            await shipmentDetailRepository.UpdateAsync(shipment.ShipmentDetails.ToArray(), cancellationToken);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(new DeleteShipmentCommandResult { Id = shipment.Id });
    }
}
