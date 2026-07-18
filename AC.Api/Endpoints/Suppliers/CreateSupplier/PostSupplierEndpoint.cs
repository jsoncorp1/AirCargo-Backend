using System.Net;
using AC.Application.Abstractions.Messaging;
using AC.Application.Modules.Suppliers.Commands.CreateSupplier;
using Ardalis.ApiEndpoints;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace AC.Api.Endpoints.Suppliers.CreateSupplier;

public class PostSupplierEndPoint(IMediator mediator)
    : EndpointBaseAsync
        .WithRequest<CreateSupplierRequest>
        .WithActionResult<CreateSupplierCommandResult>
{
    [HttpPost("api/v1/core/suppliers")]
    [SwaggerOperation(Tags = ["Core / Suppliers"])]
    [ProducesResponseType((int)HttpStatusCode.Created, Type = typeof(CreateSupplierCommandResult))]
    [ProducesResponseType((int)HttpStatusCode.BadRequest, Type = typeof(ProblemDetails))]
    public override async Task<ActionResult<CreateSupplierCommandResult>> HandleAsync(
        CreateSupplierRequest request, CancellationToken cancellationToken = default)
    {
        var command = new CreateSupplierCommand
        {
            Name = request.Name,
            Description = request.Description ?? string.Empty
        };

        var result = await mediator.SendCommandAsync<CreateSupplierCommand, CreateSupplierCommandResult>(
            command, cancellationToken);

        return result.Failure
            ? BadRequest(new ProblemDetails { Title = result.ErrorKey, Detail = result.Error })
            : Created($"api/v1/core/suppliers/{result.Value.Id}", result.Value);
    }
}

public class CreateSupplierRequest
{
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
}
