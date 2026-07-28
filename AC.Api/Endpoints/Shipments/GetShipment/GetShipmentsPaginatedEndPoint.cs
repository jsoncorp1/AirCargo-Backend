using System.Net;
using AC.Application.Abstractions.Messaging;
using AC.Application.Abstractions.Security;
using AC.Application.Modules.Shipments.Queries.GetShipmentsPaginated;
using AC.Domain.Modules.Shipments;
using AC.Domain.Modules.Roles;
using Ardalis.ApiEndpoints;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace AC.Api.Endpoints.Shipments.GetShipment;

[Authorize(Roles = RoleNames.Todos)]
public class GetShipmentsPaginatedEndPoint(IMediator mediator, ICurrentUser currentUser)
    : EndpointBaseAsync
        .WithRequest<GetShipmentsPaginatedRequest>
        .WithActionResult<GetShipmentsPaginatedQueryResult>
{
    [HttpGet("api/v1/core/shipments")]
    [SwaggerOperation(Tags = ["Core / Shipments"])]
    [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(GetShipmentsPaginatedQueryResult))]
    [ProducesResponseType((int)HttpStatusCode.BadRequest, Type = typeof(ProblemDetails))]
    public override async Task<ActionResult<GetShipmentsPaginatedQueryResult>> HandleAsync(
        [FromQuery] GetShipmentsPaginatedRequest request,
        CancellationToken cancellationToken = default)
    {
        var result = await mediator.SendQueryAsync<GetShipmentsPaginatedQuery, GetShipmentsPaginatedQueryResult>(
            new GetShipmentsPaginatedQuery
            {
                Page = request.Page,
                PerPage = request.PerPage,
                UserId = currentUser.UserId!.Value,
                SupplierId = request.SupplierId,
                OriginBranchOfficeId = request.OriginBranchOfficeId,
                DestinationBranchOfficeId = request.DestinationBranchOfficeId,
                Status = request.Status,
                DateFrom = request.DateFrom,
                DateTo = request.DateTo
            },
            cancellationToken);

        return result.Failure ? this.ToProblem(result) : Ok(result.Value);
    }
}

public class GetShipmentsPaginatedRequest
{
    public int Page { get; set; } = 1;
    public int PerPage { get; set; } = 10;
    public Guid? SupplierId { get; set; }
    public Guid? OriginBranchOfficeId { get; set; }
    public Guid? DestinationBranchOfficeId { get; set; }
    public ShipmentStatus? Status { get; set; }

    // Rango por fecha de creación del envío; ambas inclusive.
    public DateTime? DateFrom { get; set; }
    public DateTime? DateTo { get; set; }
}
