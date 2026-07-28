using System.Net;
using AC.Application.Abstractions.Messaging;
using AC.Application.Abstractions.Security;
using AC.Application.Modules.OrderDeliveries.Queries.GetOrderDeliveryById;
using AC.Domain.Modules.Roles;
using Ardalis.ApiEndpoints;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace AC.Api.Endpoints.OrderDeliveries.GetOrderDelivery;

[Authorize(Roles = RoleNames.SuperAdminAdminUsuarioEmpresa)]
public class GetOrderDeliveryByIdEndPoint(IMediator mediator, ICurrentUser currentUser)
    : EndpointBaseAsync
        .WithRequest<Guid>
        .WithActionResult<GetOrderDeliveryByIdQueryResult>
{
    [HttpGet("api/v1/core/order-deliveries/{id:guid}")]
    [SwaggerOperation(Tags = ["Core / Order Deliveries"])]
    [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(GetOrderDeliveryByIdQueryResult))]
    [ProducesResponseType((int)HttpStatusCode.NotFound, Type = typeof(ProblemDetails))]
    [ProducesResponseType((int)HttpStatusCode.Forbidden, Type = typeof(ProblemDetails))]
    public override async Task<ActionResult<GetOrderDeliveryByIdQueryResult>> HandleAsync(
        [FromRoute] Guid id, CancellationToken cancellationToken = default)
    {
        var result = await mediator.SendQueryAsync<GetOrderDeliveryByIdQuery, GetOrderDeliveryByIdQueryResult>(
            new GetOrderDeliveryByIdQuery { Id = id, UserId = currentUser.UserId!.Value }, cancellationToken);

        return result.Failure ? this.ToProblem(result) : Ok(result.Value);
    }
}
