using System.Net;
using AC.Application.Abstractions.Messaging;
using AC.Application.Abstractions.Security;
using AC.Application.Modules.OrderDeliveries.Queries.GetOrderDeliveriesPaginated;
using AC.Domain.Modules.Roles;
using Ardalis.ApiEndpoints;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace AC.Api.Endpoints.OrderDeliveries.GetOrderDelivery;

[Authorize(Roles = RoleNames.SuperAdminAdminUsuarioEmpresa)]
public class GetOrderDeliveriesPaginatedEndPoint(IMediator mediator, ICurrentUser currentUser)
    : EndpointBaseAsync
        .WithRequest<GetOrderDeliveriesPaginatedRequest>
        .WithActionResult<GetOrderDeliveriesPaginatedQueryResult>
{
    [HttpGet("api/v1/core/order-deliveries")]
    [SwaggerOperation(Tags = ["Core / Order Deliveries"])]
    [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(GetOrderDeliveriesPaginatedQueryResult))]
    [ProducesResponseType((int)HttpStatusCode.BadRequest, Type = typeof(ProblemDetails))]
    public override async Task<ActionResult<GetOrderDeliveriesPaginatedQueryResult>> HandleAsync(
        [FromQuery] GetOrderDeliveriesPaginatedRequest request,
        CancellationToken cancellationToken = default)
    {
        var result = await mediator.SendQueryAsync<GetOrderDeliveriesPaginatedQuery, GetOrderDeliveriesPaginatedQueryResult>(
            new GetOrderDeliveriesPaginatedQuery
            {
                Page = request.Page,
                PerPage = request.PerPage,
                UserId = currentUser.UserId!.Value,
                SupplierId = request.SupplierId,
                Unattended = request.Unattended
            },
            cancellationToken);

        return result.Failure ? this.ToProblem(result) : Ok(result.Value);
    }
}

public class GetOrderDeliveriesPaginatedRequest
{
    public int Page { get; set; } = 1;
    public int PerPage { get; set; } = 10;
    public Guid? SupplierId { get; set; }
    public bool? Unattended { get; set; }
}
