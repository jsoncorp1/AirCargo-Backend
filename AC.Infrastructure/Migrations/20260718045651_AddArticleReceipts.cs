using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AC.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddArticleReceipts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "article_receipts",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    count = table.Column<int>(type: "integer", nullable: false),
                    article_id = table.Column<Guid>(type: "uuid", nullable: false),
                    active = table.Column<bool>(type: "boolean", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<string>(type: "text", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    updated_by = table.Column<string>(type: "text", nullable: true),
                    deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    deleted_by = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_article_receipts", x => x.id);
                    table.ForeignKey(
                        name: "fk_article_receipts_articles_article_id",
                        column: x => x.article_id,
                        principalTable: "articles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "ix_article_receipts_article_id",
                table: "article_receipts",
                column: "article_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "article_receipts");
        }
    }
}
