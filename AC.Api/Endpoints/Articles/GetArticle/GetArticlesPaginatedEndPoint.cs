using System.Net;
using AC.Application.Abstractions.Messaging;
using AC.Application.Abstractions.Security;
using AC.Application.Modules.Articles.Queries.GetArticlesPaginated;
using AC.Domain.Modules.Roles;
using Ardalis.ApiEndpoints;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace AC.Api.Endpoints.Articles.GetArticle;

[Authorize(Roles = RoleNames.SuperAdminAdminUsuarioEmpresa)]
public class GetArticlesPaginatedEndPoint(IMediator mediator, ICurrentUser currentUser)
    : EndpointBaseAsync
        .WithRequest<GetArticlesPaginatedRequest>
        .WithActionResult<GetArticlesPaginatedQueryResult>
{
    [HttpGet("api/v1/core/articles")]
    [SwaggerOperation(Tags = ["Core / Articles"])]
    [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(GetArticlesPaginatedQueryResult))]
    [ProducesResponseType((int)HttpStatusCode.BadRequest, Type = typeof(ProblemDetails))]
    public override async Task<ActionResult<GetArticlesPaginatedQueryResult>> HandleAsync(
        [FromQuery] GetArticlesPaginatedRequest request,
        CancellationToken cancellationToken = default)
    {
        var result = await mediator.SendQueryAsync<GetArticlesPaginatedQuery, GetArticlesPaginatedQueryResult>(
            new GetArticlesPaginatedQuery
            {
                Page = request.Page,
                PerPage = request.PerPage,
                UserId = currentUser.UserId!.Value,
                SupplierId = request.SupplierId,
                ArticleName = request.ArticleName
            },
            cancellationToken);

        return result.Failure ? this.ToProblem(result) : Ok(result.Value);
    }
}

public class GetArticlesPaginatedRequest
{
    public int Page { get; set; } = 1;
    public int PerPage { get; set; } = 10;
    public Guid? SupplierId { get; set; }
    public string? ArticleName { get; set; }
}
