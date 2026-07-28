using System.Net;
using AC.Application.Abstractions.Messaging;
using AC.Application.Modules.BranchOffices.Queries.GetBranchOfficeById;
using Ardalis.ApiEndpoints;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace AC.Api.Endpoints.BranchOffices.GetBranchOffice;

public class GetBranchOfficeByIdEndPoint(IMediator mediator)
    : EndpointBaseAsync
        .WithRequest<Guid>
        .WithActionResult<GetBranchOfficeByIdQueryResult>
{
    [HttpGet("api/v1/core/branch-offices/{id:guid}")]
    [SwaggerOperation(Tags = ["Core / BranchOffices"])]
    [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(GetBranchOfficeByIdQueryResult))]
    [ProducesResponseType((int)HttpStatusCode.NotFound, Type = typeof(ProblemDetails))]
    public override async Task<ActionResult<GetBranchOfficeByIdQueryResult>> HandleAsync(
        [FromRoute] Guid id, CancellationToken cancellationToken = default)
    {
        var result = await mediator.SendQueryAsync<GetBranchOfficeByIdQuery, GetBranchOfficeByIdQueryResult>(
            new GetBranchOfficeByIdQuery { Id = id }, cancellationToken);

        return result.Failure
            ? NotFound(new ProblemDetails { Title = result.ErrorKey, Detail = result.Error })
            : Ok(result.Value);
    }
}
