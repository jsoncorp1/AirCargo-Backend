using AC.Application.Abstractions.Messaging.Commands;

namespace AC.Application.Modules.BranchOffices.Commands.UpdateBranchOffice;

public class UpdateBranchOfficeCommandResult : ICommandResult
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty;
}
