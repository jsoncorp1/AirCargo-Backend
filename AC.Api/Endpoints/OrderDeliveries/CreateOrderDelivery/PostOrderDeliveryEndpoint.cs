using System.Net;
using AC.Application.Abstractions.Messaging;
using AC.Application.Modules.OrderDeliveries.Commands.CreateOrderDelivery;
using AC.Domain.Modules.OrderDeliveries;
using Ardalis.ApiEndpoints;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace AC.Api.Endpoints.OrderDeliveries.CreateOrderDelivery;

public class PostOrderDeliveryEndPoint(IMediator mediator)
    : EndpointBaseAsync
        .WithRequest<CreateOrderDeliveryRequest>
        .WithActionResult<CreateOrderDeliveryCommandResult>
{
    [HttpPost("api/v1/core/order-deliveries")]
    [SwaggerOperation(Tags = ["Core / Order Deliveries"])]
    [ProducesResponseType((int)HttpStatusCode.Created, Type = typeof(CreateOrderDeliveryCommandResult))]
    [ProducesResponseType((int)HttpStatusCode.BadRequest, Type = typeof(ProblemDetails))]
    public override async Task<ActionResult<CreateOrderDeliveryCommandResult>> HandleAsync(
        CreateOrderDeliveryRequest request, CancellationToken cancellationToken = default)
    {
        var command = new CreateOrderDeliveryCommand
        {
            UserId = request.UserId,
            Departamento = request.Departamento,
            ClientePhone = request.ClientePhone,
            ClienteNombreCompleto = request.ClienteNombreCompleto,
            ClienteDireccion = request.ClienteDireccion,
            TipoEntrega = request.TipoEntrega,
            Lines = request.Lines.Select(l => new CreateOrderDeliveryLine
            {
                ArticleId = l.ArticleId,
                Quantity = l.Quantity,
                UnitPrice = l.UnitPrice
            }).ToList()
        };

        var result = await mediator.SendCommandAsync<CreateOrderDeliveryCommand, CreateOrderDeliveryCommandResult>(
            command, cancellationToken);

        return result.Failure
            ? BadRequest(new ProblemDetails { Title = result.ErrorKey, Detail = result.Error })
            : Created($"api/v1/core/order-deliveries/{result.Value.Id}", result.Value);
    }
}

public class CreateOrderDeliveryRequest
{
    public Guid UserId { get; set; }
    public DepartamentoBolivia Departamento { get; set; }
    public string ClientePhone { get; set; } = null!;
    public string ClienteNombreCompleto { get; set; } = null!;
    public string ClienteDireccion { get; set; } = null!;
    public TipoEntrega TipoEntrega { get; set; }
    public List<CreateOrderDeliveryLineRequest> Lines { get; set; } = [];
}

public class CreateOrderDeliveryLineRequest
{
    public Guid ArticleId { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
}
