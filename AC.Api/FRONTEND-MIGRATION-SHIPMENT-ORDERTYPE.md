# Listado de envíos: filtro por rango de fechas

`GET /api/v1/core/shipments` acepta dos parámetros nuevos para filtrar por la
**fecha de creación del envío**: `dateFrom` y `dateTo`. Ambos son opcionales e
independientes (se puede mandar solo uno) y se combinan con los filtros que ya
existían (`supplierId`, `originBranchOfficeId`, `destinationBranchOfficeId`,
`status`, `page`, `perPage`).

## Parámetros

| Parámetro | Tipo | Descripción |
|---|---|---|
| `dateFrom` | fecha o fecha-hora | Desde. Trae los envíos creados **a partir** de ese momento (inclusive). |
| `dateTo` | fecha o fecha-hora | Hasta. Trae los envíos creados **hasta** ese momento (inclusive). |

**Formato**: `yyyy-MM-dd` (recomendado) o ISO 8601 completo
(`yyyy-MM-ddTHH:mm:ssZ`).

**Ambos extremos son inclusive.** Si mandás solo la fecha (sin hora), `dateTo`
cubre el **día completo**: `dateTo=2026-07-28` incluye los envíos creados a
cualquier hora del 28.

> Las fechas se interpretan en **UTC**. Si mandás una fecha sin zona horaria se
> asume UTC; si necesitás el día calendario local, mandá el ISO completo con
> offset (ej. `2026-07-28T00:00:00-04:00`) y el backend lo convierte.

## Ejemplos

```
# envíos de un día puntual
GET /api/v1/core/shipments?dateFrom=2026-07-28&dateTo=2026-07-28

# rango de fechas
GET /api/v1/core/shipments?dateFrom=2026-07-01&dateTo=2026-07-31

# solo desde una fecha, sin tope
GET /api/v1/core/shipments?dateFrom=2026-07-28

# combinado con los filtros que ya usás
GET /api/v1/core/shipments?destinationBranchOfficeId={miSucursal}&status=InTransit&dateFrom=2026-07-01&dateTo=2026-07-31
```

La respuesta no cambia: sigue siendo el resultado paginado de siempre
(`page`, `perPage`, `count`, `totalPages`, `data[]`), y cada ítem ya trae
`createdAt`, que es el campo por el que se filtra.

## Error

| Error key | HTTP | Cuándo |
|---|---|---|
| `shipment.daterange.invalid` | 400 | `dateFrom` es posterior a `dateTo` |

```jsonc
{
  "title": "shipment.daterange.invalid",
  "status": 400,
  "detail": "La fecha desde no puede ser mayor que la fecha hasta."
}
```

## Acción para el front

- [ ] Agregar los dos selectores de fecha (desde / hasta) al listado de envíos y
      mandarlos como `dateFrom` y `dateTo` en el query string.
- [ ] Mandar el formato `yyyy-MM-dd`; no hace falta calcular el fin del día,
      el backend ya incluye el día completo en `dateTo`.
- [ ] Validar en el formulario que "desde" no sea mayor que "hasta", o mostrar
      el mensaje del error `shipment.daterange.invalid`.
