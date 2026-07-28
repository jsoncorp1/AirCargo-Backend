using AC.Application.Abstractions.Messaging.Commands;
using AC.Domain.Modules.OrderDeliveries;

namespace AC.Application.Modules.BranchOffices.Commands.CreateBranchOffice;

public class CreateBranchOfficeCommand : ICommand<CreateBranchOfficeCommandResult>
{
    public string Code { get; set; } = string.Empty;
    public BolivianDepartment BolivianDepartment { get; set; }
    public string City { get; set; } = string.Empty;
    public string? Address { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public string Phone { get; set; } = string.Empty;
}
