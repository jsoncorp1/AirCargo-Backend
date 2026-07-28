using System.Net;
using AC.Application.Abstractions.Messaging;
using AC.Application.Modules.BranchOffices.Commands.UpdateBranchOffice;
using AC.Domain.Modules.OrderDeliveries;
using AC.Domain.Modules.Roles;
using Ardalis.ApiEndpoints;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace AC.Api.Endpoints.BranchOffices.UpdateBranchOffice;

[Authorize(Roles = RoleNames.SuperAdmin)]
public class PutBranchOfficeEndPoint(IMediator mediator)
    : EndpointBaseAsync
        .WithRequest<PutBranchOfficeRequest>
        .WithActionResult<UpdateBranchOfficeCommandResult>
{
    [HttpPut("api/v1/core/branch-offices/{id:guid}")]
    [SwaggerOperation(Tags = ["Core / BranchOffices"])]
    [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(UpdateBranchOfficeCommandResult))]
    [ProducesResponseType((int)HttpStatusCode.BadRequest, Type = typeof(ProblemDetails))]
    [ProducesResponseType((int)HttpStatusCode.NotFound, Type = typeof(ProblemDetails))]
    public override async Task<ActionResult<UpdateBranchOfficeCommandResult>> HandleAsync(
        PutBranchOfficeRequest request, CancellationToken cancellationToken = default)
    {
        var command = new UpdateBranchOfficeCommand
        {
            Id = request.Id,
            Code = request.Body.Code,
            BolivianDepartment = request.Body.BolivianDepartment,
            City = request.Body.City,
            Address = request.Body.Address,
            Latitude = request.Body.Latitude,
            Longitude = request.Body.Longitude,
            Phone = request.Body.Phone
        };

        var result = await mediator.SendCommandAsync<UpdateBranchOfficeCommand, UpdateBranchOfficeCommandResult>(
            command, cancellationToken);

        if (result.Failure)
        {
            var problem = new ProblemDetails { Title = result.ErrorKey, Detail = result.Error };
            return result.ErrorKey == "branchoffice.notfound" ? NotFound(problem) : BadRequest(problem);
        }

        return Ok(result.Value);
    }
}

public class PutBranchOfficeRequest
{
    [FromRoute(Name = "id")]
    public Guid Id { get; set; }

    [FromBody]
    public PutBranchOfficeBody Body { get; set; } = new();
}

public class PutBranchOfficeBody
{
    public string Code { get; set; } = null!;
    public BolivianDepartment BolivianDepartment { get; set; }
    public string City { get; set; } = null!;
    public string? Address { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public string Phone { get; set; } = null!;
}
