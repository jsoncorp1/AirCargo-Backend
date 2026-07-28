using System.Net;
using AC.Application.Abstractions.Messaging;
using AC.Application.Abstractions.Security;
using AC.Application.Modules.ArticleReceipts.Queries.GetArticleReceiptById;
using AC.Domain.Modules.Roles;
using Ardalis.ApiEndpoints;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace AC.Api.Endpoints.ArticleReceipts.GetArticleReceipt;

[Authorize(Roles = RoleNames.SuperAdminAdminUsuarioEmpresa)]
public class GetArticleReceiptByIdEndPoint(IMediator mediator, ICurrentUser currentUser)
    : EndpointBaseAsync
        .WithRequest<Guid>
        .WithActionResult<GetArticleReceiptByIdQueryResult>
{
    [HttpGet("api/v1/core/article-receipts/{id:guid}")]
    [SwaggerOperation(Tags = ["Core / Article Receipts"])]
    [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(GetArticleReceiptByIdQueryResult))]
    [ProducesResponseType((int)HttpStatusCode.NotFound, Type = typeof(ProblemDetails))]
    [ProducesResponseType((int)HttpStatusCode.Forbidden, Type = typeof(ProblemDetails))]
    public override async Task<ActionResult<GetArticleReceiptByIdQueryResult>> HandleAsync(
        [FromRoute] Guid id, CancellationToken cancellationToken = default)
    {
        var result = await mediator.SendQueryAsync<GetArticleReceiptByIdQuery, GetArticleReceiptByIdQueryResult>(
            new GetArticleReceiptByIdQuery { Id = id, UserId = currentUser.UserId!.Value }, cancellationToken);

        return result.Failure ? this.ToProblem(result) : Ok(result.Value);
    }
}
