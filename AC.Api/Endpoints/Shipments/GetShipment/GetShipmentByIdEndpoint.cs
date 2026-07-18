using System.Net;
using AC.Application.Abstractions.Messaging;
using AC.Application.Modules.Shipments.Queries.GetShipmentById;
using Ardalis.ApiEndpoints;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace AC.Api.Endpoints.Shipments.GetShipment;

public class GetShipmentByIdEndPoint(IMediator mediator)
    : EndpointBaseAsync
        .WithRequest<Guid>
        .WithActionResult<GetShipmentByIdQueryResult>
{
    [HttpGet("api/v1/core/shipments/{id:guid}")]
    [SwaggerOperation(Tags = ["Core / Shipments"])]
    [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(GetShipmentByIdQueryResult))]
    [ProducesResponseType((int)HttpStatusCode.NotFound, Type = typeof(ProblemDetails))]
    public override async Task<ActionResult<GetShipmentByIdQueryResult>> HandleAsync(
        [FromRoute] Guid id, CancellationToken cancellationToken = default)
    {
        var result = await mediator.SendQueryAsync<GetShipmentByIdQuery, GetShipmentByIdQueryResult>(
            new GetShipmentByIdQuery { Id = id }, cancellationToken);

        return result.Failure
            ? NotFound(new ProblemDetails { Title = result.ErrorKey, Detail = result.Error })
            : Ok(result.Value);
    }
}
