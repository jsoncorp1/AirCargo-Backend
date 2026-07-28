using AC.Application.Abstractions.Messaging.Commands;

namespace AC.Application.Modules.BranchOffices.Commands.DeleteBranchOffice;

public class DeleteBranchOfficeCommandResult : ICommandResult
{
    public Guid Id { get; set; }
}
