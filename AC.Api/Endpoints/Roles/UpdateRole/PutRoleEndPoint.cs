using System.Net;
using AC.Application.Abstractions.Messaging;
using AC.Application.Modules.Roles.Commands.UpdateRole;
using Ardalis.ApiEndpoints;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace AC.Api.Endpoints.Roles.UpdateRole;

public class PutRoleEndPoint(IMediator mediator)
    : EndpointBaseAsync
        .WithRequest<PutRoleRequest>
        .WithActionResult<UpdateRoleCommandResult>
{
    [HttpPut("api/v1/core/roles/{id:guid}")]
    [SwaggerOperation(Tags = ["Core / Roles"])]
    [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(UpdateRoleCommandResult))]
    [ProducesResponseType((int)HttpStatusCode.BadRequest, Type = typeof(ProblemDetails))]
    [ProducesResponseType((int)HttpStatusCode.NotFound, Type = typeof(ProblemDetails))]
    public override async Task<ActionResult<UpdateRoleCommandResult>> HandleAsync(
        PutRoleRequest request, CancellationToken cancellationToken = default)
    {
        var command = new UpdateRoleCommand
        {
            Id = request.Id,
            Name = request.Body.Name,
            Description = request.Body.Description ?? string.Empty
        };

        var result = await mediator.SendCommandAsync<UpdateRoleCommand, UpdateRoleCommandResult>(
            command, cancellationToken);

        if (result.Failure)
        {
            var problem = new ProblemDetails { Title = result.ErrorKey, Detail = result.Error };
            return result.ErrorKey == "role.notfound" ? NotFound(problem) : BadRequest(problem);
        }

        return Ok(result.Value);
    }
}

public class PutRoleRequest
{
    [FromRoute(Name = "id")]
    public Guid Id { get; set; }

    [FromBody]
    public PutRoleBody Body { get; set; } = new();
}

public class PutRoleBody
{
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
}