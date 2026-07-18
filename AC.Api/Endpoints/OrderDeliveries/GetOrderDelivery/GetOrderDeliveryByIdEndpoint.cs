using System.Net;
using AC.Application.Abstractions.Messaging;
using AC.Application.Modules.OrderDeliveries.Queries.GetOrderDeliveryById;
using Ardalis.ApiEndpoints;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace AC.Api.Endpoints.OrderDeliveries.GetOrderDelivery;

public class GetOrderDeliveryByIdEndPoint(IMediator mediator)
    : EndpointBaseAsync
        .WithRequest<Guid>
        .WithActionResult<GetOrderDeliveryByIdQueryResult>
{
    [HttpGet("api/v1/core/order-deliveries/{id:guid}")]
    [SwaggerOperation(Tags = ["Core / Order Deliveries"])]
    [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(GetOrderDeliveryByIdQueryResult))]
    [ProducesResponseType((int)HttpStatusCode.NotFound, Type = typeof(ProblemDetails))]
    public override async Task<ActionResult<GetOrderDeliveryByIdQueryResult>> HandleAsync(
        [FromRoute] Guid id, CancellationToken cancellationToken = default)
    {
        var result = await mediator.SendQueryAsync<GetOrderDeliveryByIdQuery, GetOrderDeliveryByIdQueryResult>(
            new GetOrderDeliveryByIdQuery { Id = id }, cancellationToken);

        return result.Failure
            ? NotFound(new ProblemDetails { Title = result.ErrorKey, Detail = result.Error })
            : Ok(result.Value);
    }
}
