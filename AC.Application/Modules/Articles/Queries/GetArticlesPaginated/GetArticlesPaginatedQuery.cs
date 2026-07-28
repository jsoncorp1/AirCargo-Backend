using AC.Application.Abstractions.Messaging.Queries;

namespace AC.Application.Modules.Articles.Queries.GetArticlesPaginated;

public class GetArticlesPaginatedQuery : IQuery<GetArticlesPaginatedQueryResult>
{
    public int Page { get; set; } = 1;
    public int PerPage { get; set; } = 10;

    // Usuario autenticado que consulta; define el alcance según su rol.
    public Guid UserId { get; set; }

    public Guid? SupplierId { get; set; }
    public string? ArticleName { get; set; }
}
