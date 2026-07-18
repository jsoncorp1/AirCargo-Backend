using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AC.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixDeliveryTypeEnumValues : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Renombrar los miembros del enum DeliveryType (Pagada -> Prepaid,
            // PorPagar -> CashOnDelivery) no actualiza el texto ya guardado en
            // filas existentes; hay que migrarlo a mano.
            migrationBuilder.Sql(
                "UPDATE order_deliveries SET delivery_type = 'Prepaid' WHERE delivery_type = 'Pagada';");
            migrationBuilder.Sql(
                "UPDATE order_deliveries SET delivery_type = 'CashOnDelivery' WHERE delivery_type = 'PorPagar';");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                "UPDATE order_deliveries SET delivery_type = 'Pagada' WHERE delivery_type = 'Prepaid';");
            migrationBuilder.Sql(
                "UPDATE order_deliveries SET delivery_type = 'PorPagar' WHERE delivery_type = 'CashOnDelivery';");
        }
    }
}
