using System.Net;
using AC.Application.Abstractions.Messaging;
using AC.Application.Modules.Users.Commands.UpdateUser;
using Ardalis.ApiEndpoints;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace AC.Api.Endpoints.Users.UpdateUser;

public class PutUserEndPoint(IMediator mediator)
    : EndpointBaseAsync
        .WithRequest<PutUserRequest>
        .WithActionResult<UpdateUserCommandResult>
{
    [HttpPut("api/v1/core/users/{id:guid}")]
    [SwaggerOperation(Tags = ["Core / Users"])]
    [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(UpdateUserCommandResult))]
    [ProducesResponseType((int)HttpStatusCode.BadRequest, Type = typeof(ProblemDetails))]
    [ProducesResponseType((int)HttpStatusCode.NotFound, Type = typeof(ProblemDetails))]
    public override async Task<ActionResult<UpdateUserCommandResult>> HandleAsync(
        PutUserRequest request, CancellationToken cancellationToken = default)
    {
        var command = new UpdateUserCommand
        {
            Id = request.Id,
            FullName = request.Body.FullName,
            Email = request.Body.Email,
            PhoneNumber = request.Body.PhoneNumber ?? string.Empty,
            Dni = request.Body.Dni ?? string.Empty,
            RoleId = request.Body.RoleId,
            SupplierId = request.Body.SupplierId
        };

        var result = await mediator.SendCommandAsync<UpdateUserCommand, UpdateUserCommandResult>(
            command, cancellationToken);

        if (result.Failure)
        {
            var problem = new ProblemDetails { Title = result.ErrorKey, Detail = result.Error };
            return result.ErrorKey == "user.notfound" ? NotFound(problem) : BadRequest(problem);
        }

        return Ok(result.Value);
    }
}

public class PutUserRequest
{
    [FromRoute(Name = "id")]
    public Guid Id { get; set; }

    [FromBody]
    public PutUserBody Body { get; set; } = new();
}

public class PutUserBody
{
    public string FullName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string? PhoneNumber { get; set; }
    public string? Dni { get; set; }
    public Guid RoleId { get; set; }
    public Guid? SupplierId { get; set; }
}