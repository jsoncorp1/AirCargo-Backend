using System.Net;
using AC.Application.Abstractions.Messaging;
using AC.Application.Abstractions.Security;
using AC.Application.Modules.OrderDeliveries.Commands.UpdateOrderDelivery;
using AC.Domain.Modules.OrderDeliveries;
using AC.Domain.Modules.Roles;
using Ardalis.ApiEndpoints;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace AC.Api.Endpoints.OrderDeliveries.UpdateOrderDelivery;

[Authorize(Roles = RoleNames.SuperAdminUsuarioEmpresa)]
public class PutOrderDeliveryEndPoint(IMediator mediator, ICurrentUser currentUser)
    : EndpointBaseAsync
        .WithRequest<PutOrderDeliveryRequest>
        .WithActionResult<UpdateOrderDeliveryCommandResult>
{
    [HttpPut("api/v1/core/order-deliveries/{id:guid}")]
    [SwaggerOperation(Tags = ["Core / Order Deliveries"])]
    [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(UpdateOrderDeliveryCommandResult))]
    [ProducesResponseType((int)HttpStatusCode.BadRequest, Type = typeof(ProblemDetails))]
    [ProducesResponseType((int)HttpStatusCode.NotFound, Type = typeof(ProblemDetails))]
    [ProducesResponseType((int)HttpStatusCode.Forbidden, Type = typeof(ProblemDetails))]
    public override async Task<ActionResult<UpdateOrderDeliveryCommandResult>> HandleAsync(
        PutOrderDeliveryRequest request, CancellationToken cancellationToken = default)
    {
        var command = new UpdateOrderDeliveryCommand
        {
            Id = request.Id,
            UserId = currentUser.UserId!.Value,
            DestinationDepartment = request.Body.DestinationDepartment,
            ClientPhone = request.Body.ClientPhone,
            ClientFullName = request.Body.ClientFullName,
            ClientAddress = request.Body.ClientAddress,
            DeliveryType = request.Body.DeliveryType,
            IsExpress = request.Body.IsExpress,
            Lines = request.Body.Lines.Select(l => new UpdateOrderDeliveryLine
            {
                ArticleId = l.ArticleId,
                Quantity = l.Quantity,
                UnitPrice = l.UnitPrice
            }).ToList()
        };

        var result = await mediator.SendCommandAsync<UpdateOrderDeliveryCommand, UpdateOrderDeliveryCommandResult>(
            command, cancellationToken);

        return result.Failure ? this.ToProblem(result) : Ok(result.Value);
    }
}

public class PutOrderDeliveryRequest
{
    [FromRoute(Name = "id")]
    public Guid Id { get; set; }

    [FromBody]
    public PutOrderDeliveryBody Body { get; set; } = new();
}

public class PutOrderDeliveryBody
{
    public BolivianDepartment DestinationDepartment { get; set; }
    public string ClientPhone { get; set; } = null!;
    public string ClientFullName { get; set; } = null!;
    public string ClientAddress { get; set; } = null!;
    public DeliveryType DeliveryType { get; set; }
    public bool IsExpress { get; set; }
    public List<PutOrderDeliveryLineBody> Lines { get; set; } = [];
}

public class PutOrderDeliveryLineBody
{
    public Guid ArticleId { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
}
