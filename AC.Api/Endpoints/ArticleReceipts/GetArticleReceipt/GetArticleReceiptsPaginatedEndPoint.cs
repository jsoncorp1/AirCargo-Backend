using System.Net;
using AC.Application.Abstractions.Messaging;
using AC.Application.Abstractions.Security;
using AC.Application.Modules.ArticleReceipts.Queries.GetArticleReceiptsPaginated;
using AC.Domain.Modules.Roles;
using Ardalis.ApiEndpoints;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace AC.Api.Endpoints.ArticleReceipts.GetArticleReceipt;

[Authorize(Roles = RoleNames.SuperAdminAdminUsuarioEmpresa)]
public class GetArticleReceiptsPaginatedEndPoint(IMediator mediator, ICurrentUser currentUser)
    : EndpointBaseAsync
        .WithRequest<GetArticleReceiptsPaginatedRequest>
        .WithActionResult<GetArticleReceiptsPaginatedQueryResult>
{
    [HttpGet("api/v1/core/article-receipts")]
    [SwaggerOperation(Tags = ["Core / Article Receipts"])]
    [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(GetArticleReceiptsPaginatedQueryResult))]
    [ProducesResponseType((int)HttpStatusCode.BadRequest, Type = typeof(ProblemDetails))]
    public override async Task<ActionResult<GetArticleReceiptsPaginatedQueryResult>> HandleAsync(
        [FromQuery] GetArticleReceiptsPaginatedRequest request,
        CancellationToken cancellationToken = default)
    {
        var result = await mediator.SendQueryAsync<GetArticleReceiptsPaginatedQuery, GetArticleReceiptsPaginatedQueryResult>(
            new GetArticleReceiptsPaginatedQuery
            {
                Page = request.Page,
                PerPage = request.PerPage,
                UserId = currentUser.UserId!.Value,
                ArticleId = request.ArticleId
            },
            cancellationToken);

        return result.Failure ? this.ToProblem(result) : Ok(result.Value);
    }
}

public class GetArticleReceiptsPaginatedRequest
{
    public int Page { get; set; } = 1;
    public int PerPage { get; set; } = 10;
    public Guid? ArticleId { get; set; }
}
