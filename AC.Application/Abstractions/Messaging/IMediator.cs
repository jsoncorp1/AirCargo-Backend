using AC.Application.Abstractions.Messaging.Commands;
using AC.Application.Abstractions.Messaging.Queries;
using AC.Domain.Results;

namespace AC.Application.Abstractions.Messaging;

public interface IMediator
{
    Task<Result> SendCommandAsync<TCommand>(TCommand command, CancellationToken cancellation) where TCommand : ICommand;
    Task<Result<TResult>> SendCommandAsync<TCommand, TResult>(TCommand command, CancellationToken cancellation) where TCommand : ICommand<TResult> where TResult : ICommandResult;

    Task<Result<TResult>> SendQueryAsync<TQuery, TResult>(TQuery query, CancellationToken CancellationToken) where TQuery : IQuery<TResult>;
}
