using System.Net;
using AC.Application.Abstractions.Messaging;
using AC.Application.Modules.Suppliers.Queries.GetSuppliersPaginated;
using Ardalis.ApiEndpoints;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace AC.Api.Endpoints.Suppliers.GetSupplier;

public class GetSuppliersPaginatedEndPoint(IMediator mediator)
    : EndpointBaseAsync
        .WithRequest<GetSuppliersPaginatedRequest>
        .WithActionResult<GetSuppliersPaginatedQueryResult>
{
    [HttpGet("api/v1/core/suppliers")]
    [SwaggerOperation(Tags = ["Core / Suppliers"])]
    [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(GetSuppliersPaginatedQueryResult))]
    public override async Task<ActionResult<GetSuppliersPaginatedQueryResult>> HandleAsync(
        [FromQuery] GetSuppliersPaginatedRequest request,
        CancellationToken cancellationToken = default)
    {
        var result = await mediator.SendQueryAsync<GetSuppliersPaginatedQuery, GetSuppliersPaginatedQueryResult>(
            new GetSuppliersPaginatedQuery { Page = request.Page, PerPage = request.PerPage },
            cancellationToken);

        return Ok(result.Value);
    }
}

public class GetSuppliersPaginatedRequest
{
    public int Page { get; set; } = 1;
    public int PerPage { get; set; } = 10;
}
