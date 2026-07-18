using AC.Application.Abstractions.Messaging.Queries;

namespace AC.Application.Modules.Articles.Queries.GetArticleById;

public class GetArticleByIdQuery : IQuery<GetArticleByIdQueryResult>
{
    public Guid Id { get; set; }
}
