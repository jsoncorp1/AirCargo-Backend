using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AC.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SimplifyShipmentCodeFormat : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // El código de guía deja de incluir el segmento de forma de pago
            // (PAG/CXC): "COR-PAG-000123" pasa a "COR-000123". Solo toca filas
            // que ya tengan ese formato viejo, así es seguro re-correr.
            migrationBuilder.Sql(@"
                UPDATE shipments
                SET code = regexp_replace(code, '-(PAG|CXC)-', '-')
                WHERE code ~ '-(PAG|CXC)-';
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Reconstruye el segmento de forma de pago a partir del
            // delivery_type de la orden asociada (mismo criterio que la
            // migración BackfillShipmentCode original).
            migrationBuilder.Sql(@"
                UPDATE shipments s
                SET code = split_part(s.code, '-', 1) || '-' ||
                    CASE od.delivery_type
                        WHEN 'Prepaid' THEN 'PAG'
                        ELSE 'CXC'
                    END
                    || '-' || split_part(s.code, '-', 2)
                FROM order_deliveries od
                WHERE s.order_delivery_id = od.id
                  AND s.code ~ '^(COR|ESP)-\d+$';
            ");
        }
    }
}
