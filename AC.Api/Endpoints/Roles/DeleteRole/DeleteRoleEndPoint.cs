using System.Net;
using AC.Application.Abstractions.Messaging;
using AC.Application.Modules.Roles.Commands.DeleteRole;
using Ardalis.ApiEndpoints;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace AC.Api.Endpoints.Roles.DeleteRole;

public class DeleteRoleEndPoint(IMediator mediator)
    : EndpointBaseAsync
        .WithRequest<Guid>
        .WithActionResult<DeleteRoleCommandResult>
{
    [HttpDelete("api/v1/core/roles/{id:guid}")]
    [SwaggerOperation(Tags = ["Core / Roles"])]
    [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(DeleteRoleCommandResult))]
    [ProducesResponseType((int)HttpStatusCode.NotFound, Type = typeof(ProblemDetails))]
    public override async Task<ActionResult<DeleteRoleCommandResult>> HandleAsync(
        [FromRoute] Guid id, CancellationToken cancellationToken = default)
    {
        var result = await mediator.SendCommandAsync<DeleteRoleCommand, DeleteRoleCommandResult>(
            new DeleteRoleCommand { Id = id }, cancellationToken);

        return result.Failure
            ? NotFound(new ProblemDetails { Title = result.ErrorKey, Detail = result.Error })
            : Ok(result.Value);
    }
}