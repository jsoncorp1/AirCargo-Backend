using AC.Application.Abstractions.Messaging.Commands;

namespace AC.Application.Modules.Suppliers.Commands.UpdateSupplier;

public class UpdateSupplierCommandResult : ICommandResult
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
}
