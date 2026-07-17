namespace AC.Application.Abstractions.Messaging.Commands;

public interface ICommand
{
}

public interface ICommand<TResult> where TResult : ICommandResult
{

}

