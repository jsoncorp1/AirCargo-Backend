using System.Net;
using AC.Application.Abstractions.Messaging;
using AC.Application.Abstractions.Security;
using AC.Application.Modules.Articles.Queries.GetArticleById;
using AC.Domain.Modules.Roles;
using Ardalis.ApiEndpoints;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace AC.Api.Endpoints.Articles.GetArticle;

[Authorize(Roles = RoleNames.SuperAdminAdminUsuarioEmpresa)]
public class GetArticleByIdEndPoint(IMediator mediator, ICurrentUser currentUser)
    : EndpointBaseAsync
        .WithRequest<Guid>
        .WithActionResult<GetArticleByIdQueryResult>
{
    [HttpGet("api/v1/core/articles/{id:guid}")]
    [SwaggerOperation(Tags = ["Core / Articles"])]
    [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(GetArticleByIdQueryResult))]
    [ProducesResponseType((int)HttpStatusCode.NotFound, Type = typeof(ProblemDetails))]
    [ProducesResponseType((int)HttpStatusCode.Forbidden, Type = typeof(ProblemDetails))]
    public override async Task<ActionResult<GetArticleByIdQueryResult>> HandleAsync(
        [FromRoute] Guid id, CancellationToken cancellationToken = default)
    {
        var result = await mediator.SendQueryAsync<GetArticleByIdQuery, GetArticleByIdQueryResult>(
            new GetArticleByIdQuery { Id = id, UserId = currentUser.UserId!.Value }, cancellationToken);

        return result.Failure ? this.ToProblem(result) : Ok(result.Value);
    }
}
