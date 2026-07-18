using System.Net;
using AC.Application.Abstractions.Messaging;
using AC.Application.Modules.Articles.Queries.GetArticlesPaginated;
using Ardalis.ApiEndpoints;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace AC.Api.Endpoints.Articles.GetArticle;

public class GetArticlesPaginatedEndPoint(IMediator mediator)
    : EndpointBaseAsync
        .WithRequest<GetArticlesPaginatedRequest>
        .WithActionResult<GetArticlesPaginatedQueryResult>
{
    [HttpGet("api/v1/core/articles")]
    [SwaggerOperation(Tags = ["Core / Articles"])]
    [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(GetArticlesPaginatedQueryResult))]
    public override async Task<ActionResult<GetArticlesPaginatedQueryResult>> HandleAsync(
        [FromQuery] GetArticlesPaginatedRequest request,
        CancellationToken cancellationToken = default)
    {
        var result = await mediator.SendQueryAsync<GetArticlesPaginatedQuery, GetArticlesPaginatedQueryResult>(
            new GetArticlesPaginatedQuery
            {
                Page = request.Page,
                PerPage = request.PerPage,
                SupplierId = request.SupplierId
            },
            cancellationToken);

        return Ok(result.Value);
    }
}

public class GetArticlesPaginatedRequest
{
    public int Page { get; set; } = 1;
    public int PerPage { get; set; } = 10;
    public Guid? SupplierId { get; set; }
}
