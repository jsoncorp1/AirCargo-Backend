using AC.Application.Abstractions.Messaging.Commands;

namespace AC.Application.Modules.Articles.Commands.UpdateArticle;

public class UpdateArticleCommand : ICommand<UpdateArticleCommandResult>
{
    public Guid Id { get; set; }
    public string Sku { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public Guid SupplierId { get; set; }
}
