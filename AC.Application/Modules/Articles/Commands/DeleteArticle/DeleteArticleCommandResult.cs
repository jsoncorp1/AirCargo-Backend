using AC.Application.Abstractions.Messaging.Commands;

namespace AC.Application.Modules.Articles.Commands.DeleteArticle;

public class DeleteArticleCommandResult : ICommandResult
{
    public Guid Id { get; set; }
}
