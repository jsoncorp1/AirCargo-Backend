using AC.Domain.Modules.Shipments;
using AC.Domain.Specifications;
using Ardalis.Specification;

namespace AC.Application.Modules.Shipments.Specifications;

public class ShipmentPaginationSpecification : PaginationSpecification<Shipment>
{
    public ShipmentPaginationSpecification(
        int page,
        int perPage,
        Guid? supplierId = null,
        Guid? originBranchOfficeId = null,
        Guid? destinationBranchOfficeId = null,
        ShipmentStatus? status = null)
        : base(page, perPage)
    {
        Query
            .Include(s => s.OrderDelivery)
            .Include(s => s.OriginBranchOffice)
            .Include(s => s.DestinationBranchOffice)
            .OrderByDescending(s => s.CreatedAt);

        if (supplierId is not null)
            Query.Where(s => s.OrderDelivery.SupplierId == supplierId);

        if (originBranchOfficeId is not null)
            Query.Where(s => s.OriginBranchOfficeId == originBranchOfficeId);

        if (destinationBranchOfficeId is not null)
            Query.Where(s => s.DestinationBranchOfficeId == destinationBranchOfficeId);

        if (status is not null)
            Query.Where(s => s.Status == status);
    }
}
