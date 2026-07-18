using System.Net;
using AC.Application.Abstractions.Messaging;
using AC.Application.Modules.Suppliers.Commands.DeleteSupplier;
using Ardalis.ApiEndpoints;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace AC.Api.Endpoints.Suppliers.DeleteSupplier;

public class DeleteSupplierEndPoint(IMediator mediator)
    : EndpointBaseAsync
        .WithRequest<Guid>
        .WithActionResult<DeleteSupplierCommandResult>
{
    [HttpDelete("api/v1/core/suppliers/{id:guid}")]
    [SwaggerOperation(Tags = ["Core / Suppliers"])]
    [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(DeleteSupplierCommandResult))]
    [ProducesResponseType((int)HttpStatusCode.NotFound, Type = typeof(ProblemDetails))]
    public override async Task<ActionResult<DeleteSupplierCommandResult>> HandleAsync(
        [FromRoute] Guid id, CancellationToken cancellationToken = default)
    {
        var result = await mediator.SendCommandAsync<DeleteSupplierCommand, DeleteSupplierCommandResult>(
            new DeleteSupplierCommand { Id = id }, cancellationToken);

        return result.Failure
            ? NotFound(new ProblemDetails { Title = result.ErrorKey, Detail = result.Error })
            : Ok(result.Value);
    }
}
