using AC.Application.Abstractions.Messaging.Queries;
using AC.Application.Modules.OrderDeliveries.Specifications;
using AC.Application.Modules.Users.Specifications;
using AC.Domain.Modules.OrderDeliveries;
using AC.Domain.Modules.Roles;
using AC.Domain.Modules.Users;
using AC.Domain.Persistence;
using AC.Domain.Results;

namespace AC.Application.Modules.OrderDeliveries.Queries.GetOrderDeliveryById;

public class GetOrderDeliveryByIdQueryHandler(
    IRepository<OrderDelivery> repository,
    IRepository<User> userRepository)
    : IQueryHandler<GetOrderDeliveryByIdQuery, GetOrderDeliveryByIdQueryResult>
{
    public async Task<Result<GetOrderDeliveryByIdQueryResult>> HandleAsync(
        GetOrderDeliveryByIdQuery query, CancellationToken cancellationToken)
    {
        var order = await repository.GetBySpecificationAsync(
            new OrderDeliveryByIdSpecification(query.Id), cancellationToken);

        if (order is null)
            return Result.Fail<GetOrderDeliveryByIdQueryResult>(
                "Orden no encontrada.", "orderdelivery.notfound");

        var actor = await userRepository.GetBySpecificationAsync(
            new UserByIdSpecification(query.UserId), cancellationToken);

        if (actor is null)
            return Result.Fail<GetOrderDeliveryByIdQueryResult>(
                "El usuario autenticado no existe.", "orderdelivery.user.notfound");

        if (actor.Role.Name == RoleNames.UsuarioEmpresa && order.SupplierId != actor.SupplierId)
            return Result.Fail<GetOrderDeliveryByIdQueryResult>(
                "La orden no pertenece al proveedor del usuario.", "orderdelivery.access.forbidden");

        if (actor.Role.Name == RoleNames.Admin
            && order.OriginDepartment != actor.BranchOffice?.BolivianDepartment)
            return Result.Fail<GetOrderDeliveryByIdQueryResult>(
                "La orden no nace en el departamento de la sucursal del usuario.",
                "orderdelivery.access.forbidden");

        return Result.Success(new GetOrderDeliveryByIdQueryResult
        {
            Id = order.Id,
            SupplierId = order.SupplierId,
            SupplierName = order.Supplier?.Name,
            UserId = order.UserId,
            UserName = order.User.FullName,
            OrderType = order.OrderType.ToString(),
            OriginDepartment = order.OriginDepartment.ToString(),
            SenderFullName = order.SenderFullName,
            SenderPhone = order.SenderPhone,
            SenderAddress = order.SenderAddress,
            DestinationDepartment = order.DestinationDepartment.ToString(),
            ClientPhone = order.ClientPhone,
            ClientFullName = order.ClientFullName,
            ClientAddress = order.ClientAddress,
            DeliveryType = order.DeliveryType.ToString(),
            IsExpress = order.IsExpress,
            TotalPrice = order.TotalPrice,
            IsAttended = order.Shipments.Any(s => s.Active),
            CreatedAt = order.CreatedAt,
            Details = order.OrderDeliveryDetails.Select(d => new OrderDeliveryDetailItem
            {
                Id = d.Id,
                ArticleId = d.ArticleId,
                ArticleName = d.ArticleName,
                Quantity = d.Quantity,
                UnitPrice = d.UnitPrice,
                LineTotal = d.LineTotal
            }).ToList()
        });
    }
}
