using AC.Application.Abstractions.Messaging.Queries;
using AC.Domain.Modules.OrderDeliveries;

namespace AC.Application.Modules.BranchOffices.Queries.GetBranchOfficesPaginated;

public class GetBranchOfficesPaginatedQueryResult : IQueryResult
{
    public int Page { get; set; }
    public int PerPage { get; set; }
    public int TotalPages { get; set; }
    public int Count { get; set; }
    public IEnumerable<BranchOfficePaginatedItem> Data { get; set; } = [];
}

public class BranchOfficePaginatedItem
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public BolivianDepartment BolivianDepartment { get; set; }
    public string City { get; set; } = string.Empty;
    public string? Address { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public string Phone { get; set; } = string.Empty;
    public bool Active { get; set; }
}
