using AC.Application.Abstractions.Messaging.Commands;

namespace AC.Application.Modules.Suppliers.Commands.DeleteSupplier;

public class DeleteSupplierCommandResult : ICommandResult
{
    public Guid Id { get; set; }
}
