using AC.Application.Abstractions.Messaging.Commands;
using AC.Application.Abstractions.Messaging.Queries;
using AC.Domain.Results;
using Microsoft.Extensions.DependencyInjection;

namespace AC.Application.Abstractions.Messaging;

public class Mediator(IServiceProvider provider) : IMediator
{
    public async Task<Result> SendCommandAsync<TCommand>(
        TCommand command, CancellationToken cancellation)
        where TCommand : ICommand
    {
        var handler = provider.GetRequiredService<ICommandHandler<TCommand>>();
        return await handler.HandleAsync(command, cancellation);
    }

    public async Task<Result<TResult>> SendCommandAsync<TCommand, TResult>(
        TCommand command, CancellationToken cancellation)
        where TCommand : ICommand<TResult>
        where TResult : ICommandResult
    {
        var handler = provider.GetRequiredService<ICommandHandler<TCommand, TResult>>();
        return await handler.HandleAsync(command, cancellation);
    }

    public async Task<Result<TResult>> SendQueryAsync<TQuery, TResult>(
        TQuery query, CancellationToken cancellationToken)
        where TQuery : IQuery<TResult>
    {
        var handler = provider.GetRequiredService<IQueryHandler<TQuery, TResult>>();
        return await handler.HandleAsync(query, cancellationToken);
    }
}