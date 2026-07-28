using System.Net;
using AC.Application.Abstractions.Messaging;
using AC.Application.Modules.BranchOffices.Commands.DeleteBranchOffice;
using AC.Domain.Modules.Roles;
using Ardalis.ApiEndpoints;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace AC.Api.Endpoints.BranchOffices.DeleteBranchOffice;

[Authorize(Roles = RoleNames.SuperAdmin)]
public class DeleteBranchOfficeEndPoint(IMediator mediator)
    : EndpointBaseAsync
        .WithRequest<Guid>
        .WithActionResult<DeleteBranchOfficeCommandResult>
{
    [HttpDelete("api/v1/core/branch-offices/{id:guid}")]
    [SwaggerOperation(Tags = ["Core / BranchOffices"])]
    [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(DeleteBranchOfficeCommandResult))]
    [ProducesResponseType((int)HttpStatusCode.NotFound, Type = typeof(ProblemDetails))]
    public override async Task<ActionResult<DeleteBranchOfficeCommandResult>> HandleAsync(
        [FromRoute] Guid id, CancellationToken cancellationToken = default)
    {
        var result = await mediator.SendCommandAsync<DeleteBranchOfficeCommand, DeleteBranchOfficeCommandResult>(
            new DeleteBranchOfficeCommand { Id = id }, cancellationToken);

        return result.Failure
            ? NotFound(new ProblemDetails { Title = result.ErrorKey, Detail = result.Error })
            : Ok(result.Value);
    }
}
