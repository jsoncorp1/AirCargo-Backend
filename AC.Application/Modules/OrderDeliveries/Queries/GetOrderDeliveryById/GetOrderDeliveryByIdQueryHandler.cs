using AC.Application.Abstractions.Messaging.Queries;
using AC.Application.Modules.OrderDeliveries.Specifications;
using AC.Domain.Modules.OrderDeliveries;
using AC.Domain.Persistence;
using AC.Domain.Results;

namespace AC.Application.Modules.OrderDeliveries.Queries.GetOrderDeliveryById;

public class GetOrderDeliveryByIdQueryHandler(IRepository<OrderDelivery> repository)
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
