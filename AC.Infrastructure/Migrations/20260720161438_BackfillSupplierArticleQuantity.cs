using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AC.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class BackfillSupplierArticleQuantity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                UPDATE suppliers s
                SET article_quantity = (
                    SELECT COUNT(*)
                    FROM articles a
                    WHERE a.supplier_id = s.id
                      AND a.active = true
                );
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                UPDATE suppliers
                SET article_quantity = 0;
            ");
        }
    }
}
