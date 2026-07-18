using AC.Application.Abstractions.Messaging.Commands;

namespace AC.Application.Modules.Suppliers.Commands.CreateSupplier;

public class CreateSupplierCommandResult : ICommandResult
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
}
