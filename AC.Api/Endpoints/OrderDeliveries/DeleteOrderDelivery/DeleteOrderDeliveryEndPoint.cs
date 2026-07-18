using System.Net;
using AC.Application.Abstractions.Messaging;
using AC.Application.Modules.OrderDeliveries.Commands.DeleteOrderDelivery;
using Ardalis.ApiEndpoints;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace AC.Api.Endpoints.OrderDeliveries.DeleteOrderDelivery;

public class DeleteOrderDeliveryEndPoint(IMediator mediator)
    : EndpointBaseAsync
        .WithRequest<Guid>
        .WithActionResult<DeleteOrderDeliveryCommandResult>
{
    [HttpDelete("api/v1/core/order-deliveries/{id:guid}")]
    [SwaggerOperation(Tags = ["Core / Order Deliveries"])]
    [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(DeleteOrderDeliveryCommandResult))]
    [ProducesResponseType((int)HttpStatusCode.BadRequest, Type = typeof(ProblemDetails))]
    [ProducesResponseType((int)HttpStatusCode.NotFound, Type = typeof(ProblemDetails))]
    public override async Task<ActionResult<DeleteOrderDeliveryCommandResult>> HandleAsync(
        [FromRoute] Guid id, CancellationToken cancellationToken = default)
    {
        var result = await mediator.SendCommandAsync<DeleteOrderDeliveryCommand, DeleteOrderDeliveryCommandResult>(
            new DeleteOrderDeliveryCommand { Id = id }, cancellationToken);

        if (result.Failure)
        {
            var problem = new ProblemDetails { Title = result.ErrorKey, Detail = result.Error };
            return result.ErrorKey == "orderdelivery.notfound" ? NotFound(problem) : BadRequest(problem);
        }

        return Ok(result.Value);
    }
}
