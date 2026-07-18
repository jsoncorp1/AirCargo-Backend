using System.Net;
using AC.Application.Abstractions.Messaging;
using AC.Application.Modules.Suppliers.Queries.GetSupplierById;
using Ardalis.ApiEndpoints;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace AC.Api.Endpoints.Suppliers.GetSupplier;

public class GetSupplierByIdEndPoint(IMediator mediator)
    : EndpointBaseAsync
        .WithRequest<Guid>
        .WithActionResult<GetSupplierByIdQueryResult>
{
    [HttpGet("api/v1/core/suppliers/{id:guid}")]
    [SwaggerOperation(Tags = ["Core / Suppliers"])]
    [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(GetSupplierByIdQueryResult))]
    [ProducesResponseType((int)HttpStatusCode.NotFound, Type = typeof(ProblemDetails))]
    public override async Task<ActionResult<GetSupplierByIdQueryResult>> HandleAsync(
        [FromRoute] Guid id, CancellationToken cancellationToken = default)
    {
        var result = await mediator.SendQueryAsync<GetSupplierByIdQuery, GetSupplierByIdQueryResult>(
            new GetSupplierByIdQuery { Id = id }, cancellationToken);

        return result.Failure
            ? NotFound(new ProblemDetails { Title = result.ErrorKey, Detail = result.Error })
            : Ok(result.Value);
    }
}
