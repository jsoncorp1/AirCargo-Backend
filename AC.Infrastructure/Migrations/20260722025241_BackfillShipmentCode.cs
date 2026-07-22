using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AC.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class BackfillShipmentCode : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Todos los shipments existentes a esta fecha son corporativos (el tipo
            // esporádico no existía aún), así que el prefijo de tipo siempre es COR.
            migrationBuilder.Sql(@"
                UPDATE shipments s
                SET code = 'COR-' ||
                    CASE od.delivery_type
                        WHEN 'Prepaid' THEN 'PAG'
                        ELSE 'CXC'
                    END
                    || '-' || LPAD(s.sequence_number::text, 6, '0')
                FROM order_deliveries od
                WHERE s.order_delivery_id = od.id;
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                UPDATE shipments
                SET code = '';
            ");
        }
    }
}
