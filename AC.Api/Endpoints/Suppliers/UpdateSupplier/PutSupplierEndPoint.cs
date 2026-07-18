using System.Net;
using AC.Application.Abstractions.Messaging;
using AC.Application.Modules.Suppliers.Commands.UpdateSupplier;
using Ardalis.ApiEndpoints;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace AC.Api.Endpoints.Suppliers.UpdateSupplier;

public class PutSupplierEndPoint(IMediator mediator)
    : EndpointBaseAsync
        .WithRequest<PutSupplierRequest>
        .WithActionResult<UpdateSupplierCommandResult>
{
    [HttpPut("api/v1/core/suppliers/{id:guid}")]
    [SwaggerOperation(Tags = ["Core / Suppliers"])]
    [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(UpdateSupplierCommandResult))]
    [ProducesResponseType((int)HttpStatusCode.BadRequest, Type = typeof(ProblemDetails))]
    [ProducesResponseType((int)HttpStatusCode.NotFound, Type = typeof(ProblemDetails))]
    public override async Task<ActionResult<UpdateSupplierCommandResult>> HandleAsync(
        PutSupplierRequest request, CancellationToken cancellationToken = default)
    {
        var command = new UpdateSupplierCommand
        {
            Id = request.Id,
            Name = request.Body.Name,
            Description = request.Body.Description ?? string.Empty
        };

        var result = await mediator.SendCommandAsync<UpdateSupplierCommand, UpdateSupplierCommandResult>(
            command, cancellationToken);

        if (result.Failure)
        {
            var problem = new ProblemDetails { Title = result.ErrorKey, Detail = result.Error };
            return result.ErrorKey == "supplier.notfound" ? NotFound(problem) : BadRequest(problem);
        }

        return Ok(result.Value);
    }
}

public class PutSupplierRequest
{
    [FromRoute(Name = "id")]
    public Guid Id { get; set; }

    [FromBody]
    public PutSupplierBody Body { get; set; } = new();
}

public class PutSupplierBody
{
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
}
