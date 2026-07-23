using AC.Application.Abstractions.Messaging.Queries;
using AC.Application.Modules.OrderDeliveries.Specifications;
using AC.Domain.Modules.OrderDeliveries;
using AC.Domain.Persistence;
using AC.Domain.Results;

namespace AC.Application.Modules.OrderDeliveries.Queries.GetOrderDeliveriesPaginated;

public class GetOrderDeliveriesPaginatedQueryHandler(IRepository<OrderDelivery> repository)
    : IQueryHandler<GetOrderDeliveriesPaginatedQuery, GetOrderDeliveriesPaginatedQueryResult>
{
    public async Task<Result<GetOrderDeliveriesPaginatedQueryResult>> HandleAsync(
        GetOrderDeliveriesPaginatedQuery query, CancellationToken cancellationToken)
    {
        int page = query.Page < 1 ? 1 : query.Page;
        int perPage = query.PerPage is < 1 or > 100 ? 10 : query.PerPage;

        var spec = new OrderDeliveryPaginationSpecification(page, perPage, query.SupplierId, query.Unattended);
        var result = await repository.GetPaginatedAsync(spec, cancellationToken);

        return Result.Success(new GetOrderDeliveriesPaginatedQueryResult
        {
            Page = result.Page,
            PerPage = result.PerPage,
            TotalPages = result.TotalPages,
            Count = result.Count,
            Data = result.Data.Select(o => new OrderDeliveryPaginatedItem
            {
                Id = o.Id,
                SupplierId = o.SupplierId,
                SupplierName = o.Supplier?.Name,
                OrderType = o.OrderType.ToString(),
                ClientFullName = o.ClientFullName,
                DestinationDepartment = o.DestinationDepartment.ToString(),
                DeliveryType = o.DeliveryType.ToString(),
                TotalPrice = o.TotalPrice,
                IsAttended = o.Shipments.Any(s => s.Active),
                CreatedAt = o.CreatedAt
            })
        });
    }
}
