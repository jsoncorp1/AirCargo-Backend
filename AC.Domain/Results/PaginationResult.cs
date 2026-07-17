namespace AC.Domain.Results;

public class PaginationResult<TEntity> where TEntity : class
{
    public int Page { get; set; }
    public int PerPage { get; set; }
    public int TotalPages { get; set; }
    public int Count { get; set; }
    public IEnumerable<TEntity> Data { get; set; } = [];
}

public record BasePaginationQuery
{
    public int Page { get; set; }
    public int PerPage { get; set; }
}
