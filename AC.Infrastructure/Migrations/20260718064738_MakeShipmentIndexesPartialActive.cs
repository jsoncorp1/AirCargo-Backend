using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AC.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class MakeShipmentIndexesPartialActive : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_shipments_correlativo",
                table: "shipments");

            migrationBuilder.DropIndex(
                name: "ix_shipments_order_delivery_id",
                table: "shipments");

            migrationBuilder.CreateIndex(
                name: "ix_shipments_correlativo",
                table: "shipments",
                column: "correlativo",
                unique: true,
                filter: "active = true");

            migrationBuilder.CreateIndex(
                name: "ix_shipments_order_delivery_id",
                table: "shipments",
                column: "order_delivery_id",
                unique: true,
                filter: "active = true");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_shipments_correlativo",
                table: "shipments");

            migrationBuilder.DropIndex(
                name: "ix_shipments_order_delivery_id",
                table: "shipments");

            migrationBuilder.CreateIndex(
                name: "ix_shipments_correlativo",
                table: "shipments",
                column: "correlativo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_shipments_order_delivery_id",
                table: "shipments",
                column: "order_delivery_id",
                unique: true);
        }
    }
}
