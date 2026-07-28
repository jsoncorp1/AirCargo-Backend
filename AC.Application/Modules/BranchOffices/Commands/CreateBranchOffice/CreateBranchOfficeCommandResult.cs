using AC.Application.Abstractions.Messaging.Commands;

namespace AC.Application.Modules.BranchOffices.Commands.CreateBranchOffice;

public class CreateBranchOfficeCommandResult : ICommandResult
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty;
}
