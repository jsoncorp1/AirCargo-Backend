using AC.Application.Abstractions.Messaging.Queries;
using AC.Application.Modules.Shipments.Specifications;
using AC.Domain.Modules.Shipments;
using AC.Domain.Persistence;
using AC.Domain.Results;

namespace AC.Application.Modules.Shipments.Queries.GetShipmentsPaginated;

public class GetShipmentsPaginatedQueryHandler(IRepository<Shipment> repository)
    : IQueryHandler<GetShipmentsPaginatedQuery, GetShipmentsPaginatedQueryResult>
{
    public async Task<Result<GetShipmentsPaginatedQueryResult>> HandleAsync(
        GetShipmentsPaginatedQuery query, CancellationToken cancellationToken)
    {
        int page = query.Page < 1 ? 1 : query.Page;
        int perPage = query.PerPage is < 1 or > 100 ? 10 : query.PerPage;

        var spec = new ShipmentPaginationSpecification(page, perPage, query.SupplierId);
        var result = await repository.GetPaginatedAsync(spec, cancellationToken);

        return Result.Success(new GetShipmentsPaginatedQueryResult
        {
            Page = result.Page,
            PerPage = result.PerPage,
            TotalPages = result.TotalPages,
            Count = result.Count,
            Data = result.Data.Select(s => new ShipmentPaginatedItem
            {
                Id = s.Id,
                OrderDeliveryId = s.OrderDeliveryId,
                WaybillNumber = s.SequenceNumber.ToString("D8"),
                ClientFullName = s.OrderDelivery.ClientFullName,
                TotalWeight = s.TotalWeight,
                ShippingPrice = s.ShippingPrice,
                CreatedAt = s.CreatedAt
            })
        });
    }
}
