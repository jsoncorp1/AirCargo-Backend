using AC.Application.Abstractions.Messaging.Commands;

namespace AC.Application.Modules.Articles.Commands.DeleteArticle;

public class DeleteArticleCommand : ICommand<DeleteArticleCommandResult>
{
    public Guid Id { get; set; }
}
