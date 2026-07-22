using AC.Application.Abstractions.Messaging.Queries;

namespace AC.Application.Modules.Suppliers.Queries.GetSupplierById;

public class GetSupplierByIdQueryResult : IQueryResult
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Department { get; set; } = string.Empty;
    public int ArticleQuantity { get; set; }
    public int UserQuantity { get; set; }
}
