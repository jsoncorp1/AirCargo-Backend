using System.Net;
using AC.Application.Abstractions.Messaging;
using AC.Application.Abstractions.Security;
using AC.Application.Modules.Users.Commands.DeleteUser;
using AC.Domain.Modules.Roles;
using Ardalis.ApiEndpoints;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace AC.Api.Endpoints.Users.DeleteUser;

[Authorize(Roles = RoleNames.SuperAdminAdmin)]
public class DeleteUserEndPoint(IMediator mediator, ICurrentUser currentUser)
    : EndpointBaseAsync
        .WithRequest<Guid>
        .WithActionResult<DeleteUserCommandResult>
{
    [HttpDelete("api/v1/core/users/{id:guid}")]
    [SwaggerOperation(Tags = ["Core / Users"])]
    [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(DeleteUserCommandResult))]
    [ProducesResponseType((int)HttpStatusCode.NotFound, Type = typeof(ProblemDetails))]
    [ProducesResponseType((int)HttpStatusCode.Forbidden, Type = typeof(ProblemDetails))]
    public override async Task<ActionResult<DeleteUserCommandResult>> HandleAsync(
        [FromRoute] Guid id, CancellationToken cancellationToken = default)
    {
        var result = await mediator.SendCommandAsync<DeleteUserCommand, DeleteUserCommandResult>(
            new DeleteUserCommand { Id = id, ActorUserId = currentUser.UserId!.Value }, cancellationToken);

        return result.Failure ? this.ToProblem(result) : Ok(result.Value);
    }
}