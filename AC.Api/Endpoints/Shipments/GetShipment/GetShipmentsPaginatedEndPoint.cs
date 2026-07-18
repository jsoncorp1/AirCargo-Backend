using System.Net;
using AC.Application.Abstractions.Messaging;
using AC.Application.Modules.Shipments.Queries.GetShipmentsPaginated;
using Ardalis.ApiEndpoints;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace AC.Api.Endpoints.Shipments.GetShipment;

public class GetShipmentsPaginatedEndPoint(IMediator mediator)
    : EndpointBaseAsync
        .WithRequest<GetShipmentsPaginatedRequest>
        .WithActionResult<GetShipmentsPaginatedQueryResult>
{
    [HttpGet("api/v1/core/shipments")]
    [SwaggerOperation(Tags = ["Core / Shipments"])]
    [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(GetShipmentsPaginatedQueryResult))]
    public override async Task<ActionResult<GetShipmentsPaginatedQueryResult>> HandleAsync(
        [FromQuery] GetShipmentsPaginatedRequest request,
        CancellationToken cancellationToken = default)
    {
        var result = await mediator.SendQueryAsync<GetShipmentsPaginatedQuery, GetShipmentsPaginatedQueryResult>(
            new GetShipmentsPaginatedQuery
            {
                Page = request.Page,
                PerPage = request.PerPage,
                SupplierId = request.SupplierId
            },
            cancellationToken);

        return Ok(result.Value);
    }
}

public class GetShipmentsPaginatedRequest
{
    public int Page { get; set; } = 1;
    public int PerPage { get; set; } = 10;
    public Guid? SupplierId { get; set; }
}
