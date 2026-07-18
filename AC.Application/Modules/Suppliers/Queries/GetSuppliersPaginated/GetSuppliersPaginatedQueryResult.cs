using AC.Application.Abstractions.Messaging.Queries;

namespace AC.Application.Modules.Suppliers.Queries.GetSuppliersPaginated;

public class GetSuppliersPaginatedQueryResult : IQueryResult
{
    public int Page { get; set; }
    public int PerPage { get; set; }
    public int TotalPages { get; set; }
    public int Count { get; set; }
    public IEnumerable<SupplierPaginatedItem> Data { get; set; } = [];
}

public class SupplierPaginatedItem
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}
