using AC.Domain.Results;

namespace AC.Application.Abstractions.Messaging.Commands;

public interface ICommandHandler<TCommand> where TCommand : ICommand
{
    public abstract Task<Result> HandleAsync(TCommand command, CancellationToken cancellationToken);
}

public interface ICommandHandler<TCommand, TResult>
    where TCommand : ICommand<TResult>
    where TResult : ICommandResult
{
    public abstract Task<Result<TResult>> HandleAsync(TCommand command, CancellationToken cancellationToken);
}

