using System.Net;
using AC.Application.Abstractions.Messaging;
using AC.Application.Modules.Auth.Commands.Login;
using Ardalis.ApiEndpoints;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace AC.Api.Endpoints.Auth.Login;

public class PostLoginEndPoint(IMediator mediator)
    : EndpointBaseAsync
        .WithRequest<LoginRequest>
        .WithActionResult<LoginCommandResult>
{
    [HttpPost("api/v1/core/auth/login")]
    [SwaggerOperation(Tags = ["Core / Auth"])]
    [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(LoginCommandResult))]
    [ProducesResponseType((int)HttpStatusCode.BadRequest, Type = typeof(ProblemDetails))]
    public override async Task<ActionResult<LoginCommandResult>> HandleAsync(
        LoginRequest request, CancellationToken cancellationToken = default)
    {
        var command = new LoginCommand
        {
            Email = request.Email,
            Password = request.Password
        };

        var result = await mediator.SendCommandAsync<LoginCommand, LoginCommandResult>(
            command, cancellationToken);

        return result.Failure
            ? BadRequest(new ProblemDetails { Title = result.ErrorKey, Detail = result.Error })
            : Ok(result.Value);
    }
}
