using System.Net;
using AC.Application.Abstractions.Messaging;
using AC.Application.Abstractions.Security;
using AC.Application.Modules.OrderDeliveries.Commands.DeleteOrderDelivery;
using AC.Domain.Modules.Roles;
using Ardalis.ApiEndpoints;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace AC.Api.Endpoints.OrderDeliveries.DeleteOrderDelivery;

[Authorize(Roles = RoleNames.SuperAdminUsuarioEmpresa)]
public class DeleteOrderDeliveryEndPoint(IMediator mediator, ICurrentUser currentUser)
    : EndpointBaseAsync
        .WithRequest<Guid>
        .WithActionResult<DeleteOrderDeliveryCommandResult>
{
    [HttpDelete("api/v1/core/order-deliveries/{id:guid}")]
    [SwaggerOperation(Tags = ["Core / Order Deliveries"])]
    [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(DeleteOrderDeliveryCommandResult))]
    [ProducesResponseType((int)HttpStatusCode.BadRequest, Type = typeof(ProblemDetails))]
    [ProducesResponseType((int)HttpStatusCode.NotFound, Type = typeof(ProblemDetails))]
    [ProducesResponseType((int)HttpStatusCode.Forbidden, Type = typeof(ProblemDetails))]
    public override async Task<ActionResult<DeleteOrderDeliveryCommandResult>> HandleAsync(
        [FromRoute] Guid id, CancellationToken cancellationToken = default)
    {
        var result = await mediator.SendCommandAsync<DeleteOrderDeliveryCommand, DeleteOrderDeliveryCommandResult>(
            new DeleteOrderDeliveryCommand { Id = id, UserId = currentUser.UserId!.Value }, cancellationToken);

        return result.Failure ? this.ToProblem(result) : Ok(result.Value);
    }
}
