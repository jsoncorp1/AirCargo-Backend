using AC.Application.Abstractions.Messaging.Commands;

namespace AC.Application.Modules.Articles.Commands.CreateArticle;

public class CreateArticleCommandResult : ICommandResult
{
    public Guid Id { get; set; }
    public string Sku { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public int Count { get; set; }
    public decimal Price { get; set; }
    public Guid SupplierId { get; set; }
}
