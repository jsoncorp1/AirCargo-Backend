using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AC.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddOriginDestinationSenderAndPackages : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // La columna vieja "department" guardaba el destino (así la usan los
            // handlers hoy), no el origen: el rename automático de EF la habría
            // mapeado a "origin_department" por error. La renombramos a
            // "destination_department" para preservar el significado real de los
            // datos existentes.
            migrationBuilder.RenameColumn(
                name: "department",
                table: "order_deliveries",
                newName: "destination_department");

            migrationBuilder.AddColumn<int>(
                name: "package_count",
                table: "shipments",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "package_description",
                table: "shipments",
                type: "character varying(300)",
                maxLength: 300,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "origin_department",
                table: "order_deliveries",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "Beni");

            migrationBuilder.AddColumn<string>(
                name: "sender_address",
                table: "order_deliveries",
                type: "character varying(300)",
                maxLength: 300,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "sender_full_name",
                table: "order_deliveries",
                type: "character varying(150)",
                maxLength: 150,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "sender_phone",
                table: "order_deliveries",
                type: "character varying(30)",
                maxLength: 30,
                nullable: false,
                defaultValue: "");

            // Backfill: para órdenes corporativas ya existentes, el origen es el
            // departamento/nombre del proveedor, igual que hace el handler al
            // crear una orden corporativa nueva. Las esporádicas existentes no
            // tenían remitente/origen capturado antes de este cambio, así que
            // quedan con el default ("Beni" / cadena vacía) hasta que se editen.
            migrationBuilder.Sql(@"
                UPDATE order_deliveries od
                SET origin_department = s.department,
                    sender_full_name = s.name
                FROM suppliers s
                WHERE od.supplier_id = s.id
                  AND od.order_type = 'Corporate';
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "package_count",
                table: "shipments");

            migrationBuilder.DropColumn(
                name: "package_description",
                table: "shipments");

            migrationBuilder.DropColumn(
                name: "origin_department",
                table: "order_deliveries");

            migrationBuilder.DropColumn(
                name: "sender_address",
                table: "order_deliveries");

            migrationBuilder.DropColumn(
                name: "sender_full_name",
                table: "order_deliveries");

            migrationBuilder.DropColumn(
                name: "sender_phone",
                table: "order_deliveries");

            migrationBuilder.RenameColumn(
                name: "destination_department",
                table: "order_deliveries",
                newName: "department");
        }
    }
}
