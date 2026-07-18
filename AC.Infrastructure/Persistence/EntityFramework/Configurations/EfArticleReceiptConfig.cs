using AC.Domain.Modules.ArticleReceipts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AC.Infrastructure.Persistence.EntityFramework.Configurations;

internal class EfArticleReceiptConfig : IEntityTypeConfiguration<ArticleReceipt>
{
    public void Configure(EntityTypeBuilder<ArticleReceipt> builder)
    {
        // La relación N-1 con Article se arma sola por convención
        // (ArticleReceipt.ArticleId + ArticleReceipt.Article + Article.ArticleReceipts);
        // el delete queda en Restrict por el loop global del CoreDbContext.
    }
}
