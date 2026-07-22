using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AC.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class BackfillSupplierUserQuantity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                UPDATE suppliers s
                SET user_quantity = (
                    SELECT COUNT(*)
                    FROM users u
                    WHERE u.supplier_id = s.id
                      AND u.active = true
                );
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                UPDATE suppliers
                SET user_quantity = 0;
            ");
        }
    }
}
