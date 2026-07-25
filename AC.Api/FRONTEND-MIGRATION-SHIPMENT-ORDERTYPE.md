# Nuevo campo `isExpress` (envío expreso / normal)

**No rompe nada** — es un campo booleano nuevo, opcional en la práctica: si
no se manda, el backend asume `false` (envío normal).

Se agregó a la orden de envío (`OrderDelivery`) para distinguir si el envío
es **expreso** (`true`) o **normal** (`false`).

## Dónde aparece

**Se manda (request) en:**
- `POST /api/v1/core/order-deliveries`
- `PUT /api/v1/core/order-deliveries/{id}`
- `POST /api/v1/core/shipments/sporadic`

**Se devuelve (response) en:**
- Las respuestas de los tres endpoints de arriba.
- `GET /api/v1/core/order-deliveries/{id}`
- `GET /api/v1/core/order-deliveries` (listado paginado)

## Ejemplo

```jsonc
// POST /api/v1/core/order-deliveries
{
  "destinationDepartment": "LaPaz",
  "clientPhone": "...", "clientFullName": "...", "clientAddress": "...",
  "deliveryType": "Prepaid",
  "isExpress": true,          // NUEVO — true = expreso, false = normal
  "lines": [ /* ... */ ]
}
```

La respuesta (y los `GET`) devuelven el mismo campo `isExpress` junto al
resto de los datos de la orden.

## Acción para el front

- [ ] Agregar un toggle/checkbox de "Envío expreso" en los formularios de
      creación y edición de órdenes (`order-deliveries`) y en el de envío
      esporádico (`shipments/sporadic`), mandando `isExpress` en el body.
- [ ] Mostrar el valor de `isExpress` en el detalle y en el listado de
      órdenes (por ejemplo, un badge "Expreso"/"Normal").
- [ ] Si no se manda el campo, no pasa nada — el backend guarda `false` por
      defecto, pero conviene no depender de eso a futuro.
