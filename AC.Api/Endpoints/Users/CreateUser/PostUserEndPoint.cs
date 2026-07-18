using System.Net;
using AC.Application.Abstractions.Messaging;
using AC.Application.Modules.Users.Commands.CreateUser;
using Ardalis.ApiEndpoints;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace AC.Api.Endpoints.Users.CreateUser;

public class PostUserEndPoint(IMediator mediator)
    : EndpointBaseAsync
        .WithRequest<CreateUserRequest>
        .WithActionResult<CreateUserCommandResult>
{
    [HttpPost("api/v1/core/users")]
    [SwaggerOperation(Tags = ["Core / Users"])]
    [ProducesResponseType((int)HttpStatusCode.Created, Type = typeof(CreateUserCommandResult))]
    [ProducesResponseType((int)HttpStatusCode.BadRequest, Type = typeof(ProblemDetails))]
    public override async Task<ActionResult<CreateUserCommandResult>> HandleAsync(
        CreateUserRequest request, CancellationToken cancellationToken = default)
    {
        var command = new CreateUserCommand
        {
            FullName = request.FullName,
            Email = request.Email,
            Password = request.Password,
            PhoneNumber = request.PhoneNumber ?? string.Empty,
            Dni = request.Dni ?? string.Empty,
            RoleId = request.RoleId,
            SupplierId = request.SupplierId
        };

        var result = await mediator.SendCommandAsync<CreateUserCommand, CreateUserCommandResult>(
            command, cancellationToken);

        return result.Failure
            ? BadRequest(new ProblemDetails { Title = result.ErrorKey, Detail = result.Error })
            : Created($"api/v1/core/users/{result.Value.Id}", result.Value);
    }
}

