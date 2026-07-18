using AC.Application.Abstractions.Messaging.Commands;

namespace AC.Application.Modules.Articles.Commands.CreateArticle;

public class CreateArticleCommand : ICommand<CreateArticleCommandResult>
{
    public string Sku { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public Guid SupplierId { get; set; }
}
