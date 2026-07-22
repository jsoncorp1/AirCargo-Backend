using AC.Application.Abstractions.Messaging.Commands;
using AC.Application.Modules.Users.Specifications;
using AC.Domain.Modules.OrderDeliveries;
using AC.Domain.Modules.Shipments;
using AC.Domain.Modules.Users;
using AC.Domain.Persistence;
using AC.Domain.Results;

namespace AC.Application.Modules.Shipments.Commands.CreateSporadicShipment;

public class CreateSporadicShipmentCommandHandler(
    IRepository<User> userRepository,
    IRepository<OrderDelivery> orderDeliveryRepository,
    IRepository<OrderDeliveryDetail> orderDeliveryDetailRepository,
    IRepository<Shipment> shipmentRepository,
    IRepository<ShipmentDetail> shipmentDetailRepository,
    ICoreUnitOfWork unitOfWork)
    : ICommandHandler<CreateSporadicShipmentCommand, CreateSporadicShipmentCommandResult>
{
    public async Task<Result<CreateSporadicShipmentCommandResult>> HandleAsync(
        CreateSporadicShipmentCommand command, CancellationToken cancellationToken)
    {
        if (command.Lines.Count == 0)
            return Result.Fail<CreateSporadicShipmentCommandResult>(
                "El envío debe tener al menos una línea.", "sporadicshipment.lines.required");

        var user = await userRepository.GetBySpecificationAsync(
            new UserByIdSpecification(command.UserId), cancellationToken);

        if (user is null)
            return Result.Fail<CreateSporadicShipmentCommandResult>(
                "El usuario indicado no existe.", "sporadicshipment.user.notfound");

        foreach (var line in command.Lines)
        {
            if (string.IsNullOrWhiteSpace(line.ArticleName))
                return Result.Fail<CreateSporadicShipmentCommandResult>(
                    "El nombre del artículo es obligatorio.", "sporadicshipment.articlename.required");

            if (line.Quantity <= 0)
                return Result.Fail<CreateSporadicShipmentCommandResult>(
                    "La cantidad de cada línea debe ser mayor a cero.", "sporadicshipment.quantity.invalid");

            if (line.Weight <= 0)
                return Result.Fail<CreateSporadicShipmentCommandResult>(
                    "El peso de cada línea debe ser mayor a cero.", "sporadicshipment.weight.invalid");

            if (line.ShippingCost < 0)
                return Result.Fail<CreateSporadicShipmentCommandResult>(
                    "El costo de envío de cada línea no puede ser negativo.", "sporadicshipment.shippingcost.invalid");
        }

        var orderDelivery = new OrderDelivery
        {
            Id = Guid.NewGuid(),
            SupplierId = null,
            UserId = command.UserId,
            OrderType = OrderType.Sporadic,
            Department = command.Department,
            ClientPhone = command.ClientPhone,
            ClientFullName = command.ClientFullName,
            ClientAddress = command.ClientAddress,
            DeliveryType = command.DeliveryType,
            TotalPrice = command.Lines.Sum(l => l.Quantity * l.UnitPrice)
        };

        var orderDetails = command.Lines.Select(l => new OrderDeliveryDetail
        {
            Id = Guid.NewGuid(),
            OrderDeliveryId = orderDelivery.Id,
            ArticleId = null,
            ArticleName = l.ArticleName,
            Quantity = l.Quantity,
            UnitPrice = l.UnitPrice,
            LineTotal = l.Quantity * l.UnitPrice
        }).ToList();

        int sequenceNumber = (await shipmentRepository.MaxAsync(s => s.SequenceNumber, cancellationToken) ?? 0) + 1;
        string code = ShipmentCodeFormatter.Format(orderDelivery.OrderType, orderDelivery.DeliveryType, sequenceNumber);

        var shipment = new Shipment
        {
            Id = Guid.NewGuid(),
            OrderDeliveryId = orderDelivery.Id,
            SequenceNumber = sequenceNumber,
            Code = code,
            TotalWeight = command.Lines.Sum(l => l.Weight),
            ShippingPrice = command.Lines.Sum(l => l.ShippingCost)
        };

        var shipmentDetails = orderDetails.Zip(command.Lines, (detail, line) => new ShipmentDetail
        {
            Id = Guid.NewGuid(),
            ShipmentId = shipment.Id,
            OrderDeliveryDetailId = detail.Id,
            Weight = line.Weight,
            ShippingCost = line.ShippingCost
        }).ToList();

        await orderDeliveryRepository.SaveAsync(orderDelivery, cancellationToken);
        await orderDeliveryDetailRepository.SaveAsync(orderDetails.ToArray(), cancellationToken);
        await shipmentRepository.SaveAsync(shipment, cancellationToken);
        await shipmentDetailRepository.SaveAsync(shipmentDetails.ToArray(), cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(new CreateSporadicShipmentCommandResult
        {
            OrderDeliveryId = orderDelivery.Id,
            ShipmentId = shipment.Id,
            Code = shipment.Code,
            TotalPrice = orderDelivery.TotalPrice,
            TotalWeight = shipment.TotalWeight,
            ShippingPrice = shipment.ShippingPrice,
            Details = orderDetails.Zip(shipmentDetails, (od, sd) => new CreateSporadicShipmentDetailResult
            {
                OrderDeliveryDetailId = od.Id,
                ShipmentDetailId = sd.Id,
                ArticleName = od.ArticleName,
                Quantity = od.Quantity,
                UnitPrice = od.UnitPrice,
                LineTotal = od.LineTotal,
                Weight = sd.Weight,
                ShippingCost = sd.ShippingCost
            }).ToList()
        });
    }
}
