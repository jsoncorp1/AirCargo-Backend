using System.Net;
using AC.Application.Abstractions.Messaging;
using AC.Application.Modules.Roles.Commands.CreateRole;
using Ardalis.ApiEndpoints;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace AC.Api.Endpoints.Roles.CreateRole;

public class PostRoleEndPoint(IMediator mediator)
    : EndpointBaseAsync
        .WithRequest<CreateRoleRequest>
        .WithActionResult<CreateRoleCommandResult>
{
    [HttpPost("api/v1/core/roles")]
    [SwaggerOperation(Tags = ["Core / Roles"])]
    [ProducesResponseType((int)HttpStatusCode.Created, Type = typeof(CreateRoleCommandResult))]
    [ProducesResponseType((int)HttpStatusCode.BadRequest, Type = typeof(ProblemDetails))]
    public override async Task<ActionResult<CreateRoleCommandResult>> HandleAsync(
        CreateRoleRequest request, CancellationToken cancellationToken = default)
    {
        var command = new CreateRoleCommand
        {
            Name = request.Name,
            Description = request.Description ?? string.Empty
        };

        var result = await mediator.SendCommandAsync<CreateRoleCommand, CreateRoleCommandResult>(
            command, cancellationToken);

        return result.Failure
            ? BadRequest(new ProblemDetails { Title = result.ErrorKey, Detail = result.Error })
            : Created($"api/v1/core/roles/{result.Value.Id}", result.Value);
    }
}

public class CreateRoleRequest
{
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
}