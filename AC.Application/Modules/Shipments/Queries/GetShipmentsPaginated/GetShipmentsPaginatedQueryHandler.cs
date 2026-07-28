using AC.Application.Abstractions.Messaging.Queries;
using AC.Application.Modules.Shipments.Specifications;
using AC.Application.Modules.Users.Specifications;
using AC.Domain.Modules.Roles;
using AC.Domain.Modules.Shipments;
using AC.Domain.Modules.Users;
using AC.Domain.Persistence;
using AC.Domain.Results;

namespace AC.Application.Modules.Shipments.Queries.GetShipmentsPaginated;

public class GetShipmentsPaginatedQueryHandler(
    IRepository<Shipment> repository,
    IRepository<User> userRepository)
    : IQueryHandler<GetShipmentsPaginatedQuery, GetShipmentsPaginatedQueryResult>
{
    public async Task<Result<GetShipmentsPaginatedQueryResult>> HandleAsync(
        GetShipmentsPaginatedQuery query, CancellationToken cancellationToken)
    {
        int page = query.Page < 1 ? 1 : query.Page;
        int perPage = query.PerPage is < 1 or > 100 ? 10 : query.PerPage;

        var actor = await userRepository.GetBySpecificationAsync(
            new UserByIdSpecification(query.UserId), cancellationToken);

        if (actor is null)
            return Result.Fail<GetShipmentsPaginatedQueryResult>(
                "El usuario autenticado no existe.", "shipment.user.notfound");

        // El alcance se decide con datos de BD, nunca con filtros libres del cliente.
        var supplierId = query.SupplierId;
        Guid? anyBranchOfficeId = null;

        if (actor.Role.Name == RoleNames.UsuarioEmpresa)
        {
            if (actor.SupplierId is null)
                return Result.Fail<GetShipmentsPaginatedQueryResult>(
                    "El usuario no pertenece a ningún proveedor.", "shipment.user.notsupplier");

            supplierId = actor.SupplierId;
        }
        else if (actor.Role.Name is RoleNames.Admin or RoleNames.Conductor)
        {
            if (actor.BranchOfficeId is null)
                return Result.Fail<GetShipmentsPaginatedQueryResult>(
                    "El usuario no tiene una sucursal asignada.", "shipment.user.nobranch");

            anyBranchOfficeId = actor.BranchOfficeId;
        }

        var spec = new ShipmentPaginationSpecification(
            page, perPage,
            supplierId,
            query.OriginBranchOfficeId,
            query.DestinationBranchOfficeId,
            query.Status,
            anyBranchOfficeId);
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
                Code = s.Code,
                ClientFullName = s.OrderDelivery.ClientFullName,
                SupplierId = s.OrderDelivery.SupplierId,
                OriginBranchOfficeId = s.OriginBranchOfficeId,
                OriginBranchOfficeCode = s.OriginBranchOffice != null ? s.OriginBranchOffice.Code : null,
                DestinationBranchOfficeId = s.DestinationBranchOfficeId,
                DestinationBranchOfficeCode = s.DestinationBranchOffice != null ? s.DestinationBranchOffice.Code : null,
                Status = s.Status,
                Observation = s.Observation,
                TotalWeight = s.TotalWeight,
                ShippingPrice = s.ShippingPrice,
                PackageCount = s.PackageCount,
                CreatedAt = s.CreatedAt
            })
        });
    }
}
