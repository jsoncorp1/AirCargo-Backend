using AC.Domain.Results;

namespace AC.Application.Abstractions.Messaging.Queries;

public interface IQueryHandler<TQuery, TResult> where TQuery : IQuery<TResult>
{
    Task<Result<TResult>> HandleAsync(TQuery query, CancellationToken cancellationToken);
}
