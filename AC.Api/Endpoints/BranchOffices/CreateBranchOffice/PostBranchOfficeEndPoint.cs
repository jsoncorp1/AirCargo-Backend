using System.Net;
using AC.Application.Abstractions.Messaging;
using AC.Application.Modules.BranchOffices.Commands.CreateBranchOffice;
using AC.Domain.Modules.OrderDeliveries;
using Ardalis.ApiEndpoints;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace AC.Api.Endpoints.BranchOffices.CreateBranchOffice;

public class PostBranchOfficeEndPoint(IMediator mediator)
    : EndpointBaseAsync
        .WithRequest<CreateBranchOfficeRequest>
        .WithActionResult<CreateBranchOfficeCommandResult>
{
    [HttpPost("api/v1/core/branch-offices")]
    [SwaggerOperation(Tags = ["Core / BranchOffices"])]
    [ProducesResponseType((int)HttpStatusCode.Created, Type = typeof(CreateBranchOfficeCommandResult))]
    [ProducesResponseType((int)HttpStatusCode.BadRequest, Type = typeof(ProblemDetails))]
    public override async Task<ActionResult<CreateBranchOfficeCommandResult>> HandleAsync(
        CreateBranchOfficeRequest request, CancellationToken cancellationToken = default)
    {
        var command = new CreateBranchOfficeCommand
        {
            Code = request.Code,
            BolivianDepartment = request.BolivianDepartment,
            City = request.City,
            Address = request.Address,
            Latitude = request.Latitude,
            Longitude = request.Longitude,
            Phone = request.Phone
        };

        var result = await mediator.SendCommandAsync<CreateBranchOfficeCommand, CreateBranchOfficeCommandResult>(
            command, cancellationToken);

        return result.Failure
            ? BadRequest(new ProblemDetails { Title = result.ErrorKey, Detail = result.Error })
            : Created($"api/v1/core/branch-offices/{result.Value.Id}", result.Value);
    }
}

public class CreateBranchOfficeRequest
{
    public string Code { get; set; } = null!;
    public BolivianDepartment BolivianDepartment { get; set; }
    public string City { get; set; } = null!;
    public string? Address { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public string Phone { get; set; } = null!;
}
