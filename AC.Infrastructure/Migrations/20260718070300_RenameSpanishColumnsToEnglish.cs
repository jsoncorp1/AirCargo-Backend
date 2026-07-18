using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AC.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RenameSpanishColumnsToEnglish : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "correlativo",
                table: "shipments",
                newName: "sequence_number");

            migrationBuilder.RenameIndex(
                name: "ix_shipments_correlativo",
                table: "shipments",
                newName: "ix_shipments_sequence_number");

            migrationBuilder.RenameColumn(
                name: "departamento",
                table: "order_deliveries",
                newName: "department");

            migrationBuilder.RenameColumn(
                name: "tipo_entrega",
                table: "order_deliveries",
                newName: "delivery_type");

            migrationBuilder.RenameColumn(
                name: "cliente_phone",
                table: "order_deliveries",
                newName: "client_phone");

            migrationBuilder.RenameColumn(
                name: "cliente_nombre_completo",
                table: "order_deliveries",
                newName: "client_full_name");

            migrationBuilder.RenameColumn(
                name: "cliente_direccion",
                table: "order_deliveries",
                newName: "client_address");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "sequence_number",
                table: "shipments",
                newName: "correlativo");

            migrationBuilder.RenameIndex(
                name: "ix_shipments_sequence_number",
                table: "shipments",
                newName: "ix_shipments_correlativo");

            migrationBuilder.RenameColumn(
                name: "delivery_type",
                table: "order_deliveries",
                newName: "tipo_entrega");

            migrationBuilder.RenameColumn(
                name: "department",
                table: "order_deliveries",
                newName: "departamento");

            migrationBuilder.RenameColumn(
                name: "client_phone",
                table: "order_deliveries",
                newName: "cliente_phone");

            migrationBuilder.RenameColumn(
                name: "client_full_name",
                table: "order_deliveries",
                newName: "cliente_nombre_completo");

            migrationBuilder.RenameColumn(
                name: "client_address",
                table: "order_deliveries",
                newName: "cliente_direccion");
        }
    }
}
