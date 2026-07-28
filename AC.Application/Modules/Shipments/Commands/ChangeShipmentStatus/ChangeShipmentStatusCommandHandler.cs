using AC.Application.Abstractions.Messaging.Commands;
using AC.Application.Modules.Shipments.Specifications;
using AC.Application.Modules.Users.Specifications;
using AC.Domain.Modules.Roles;
using AC.Domain.Modules.Shipments;
using AC.Domain.Modules.Users;
using AC.Domain.Persistence;
using AC.Domain.Results;

namespace AC.Application.Modules.Shipments.Commands.ChangeShipmentStatus;

public class ChangeShipmentStatusCommandHandler(
    IRepository<Shipment> shipmentRepository,
    IRepository<User> userRepository,
    ICoreUnitOfWork unitOfWork)
    : ICommandHandler<ChangeShipmentStatusCommand, ChangeShipmentStatusCommandResult>
{
    public async Task<Result<ChangeShipmentStatusCommandResult>> HandleAsync(
        ChangeShipmentStatusCommand command, CancellationToken cancellationToken)
    {
        var shipment = await shipmentRepository.GetBySpecificationAsync(
            new ShipmentByIdSpecification(command.Id), cancellationToken);

        if (shipment is null)
            return Result.Fail<ChangeShipmentStatusCommandResult>(
                "Envío no encontrado.", "shipment.notfound");

        // Admin y conductor solo cambian estados de envíos de su sucursal (origen o destino).
        var actor = await userRepository.GetBySpecificationAsync(
            new UserByIdSpecification(command.UserId), cancellationToken);

        if (actor is null)
            return Result.Fail<ChangeShipmentStatusCommandResult>(
                "El usuario autenticado no existe.", "shipment.user.notfound");

        if (actor.Role.Name is RoleNames.Admin or RoleNames.Conductor)
        {
            if (actor.BranchOfficeId is null)
                return Result.Fail<ChangeShipmentStatusCommandResult>(
                    "El usuario no tiene una sucursal asignada.", "shipment.user.nobranch");

            if (shipment.OriginBranchOfficeId != actor.BranchOfficeId
                && shipment.DestinationBranchOfficeId != actor.BranchOfficeId)
                return Result.Fail<ChangeShipmentStatusCommandResult>(
                    "El envío no pertenece a la sucursal del usuario.", "shipment.statuschange.forbidden");
        }

        if (command.Status is null && command.Observation is null && command.DeliveryComment is null)
            return Result.Fail<ChangeShipmentStatusCommandResult>(
                "Debe indicar un estado, una observación o un comentario.", "shipment.statuschange.empty");

        if (command.Status is not null && command.Observation is not null)
            return Result.Fail<ChangeShipmentStatusCommandResult>(
                "No se puede indicar estado y observación a la vez; la observación define el estado.",
                "shipment.statuschange.conflict");

        if (command.Observation is not null)
        {
            if (!ShipmentStatusRules.CanReceiveObservation(shipment.Status))
                return Result.Fail<ChangeShipmentStatusCommandResult>(
                    $"No se puede observar un envío en estado {shipment.Status}.",
                    "shipment.observation.invalidstatus");

            shipment.Observation = command.Observation;
            shipment.Status = ShipmentStatusRules.StatusFor(command.Observation.Value);
        }
        else if (command.Status is not null)
        {
            if (!ShipmentStatusRules.CanTransition(shipment.Status, command.Status.Value))
                return Result.Fail<ChangeShipmentStatusCommandResult>(
                    $"Transición inválida: {shipment.Status} → {command.Status}.",
                    "shipment.statuschange.invalidtransition");

            shipment.Status = command.Status.Value;
        }

        if (command.DeliveryComment is not null)
            shipment.DeliveryComment = command.DeliveryComment;

        await shipmentRepository.UpdateAsync(shipment, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(new ChangeShipmentStatusCommandResult
        {
            Id = shipment.Id,
            Code = shipment.Code,
            Status = shipment.Status,
            Observation = shipment.Observation,
            DeliveryComment = shipment.DeliveryComment
        });
    }
}
