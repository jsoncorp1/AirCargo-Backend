# Guía de migración para el frontend — OrderType (corporativo/esporádico) y Code de guía

Esta guía resume los cambios de contrato en la API tras agregar el tipo de envío
(corporativo / esporádico) y el código de guía con prefijo. Los nombres de campo
están en **camelCase** (política `CamelCase` del backend) y los enums viajan como
**string** (`JsonStringEnumConverter` global).

## 1. Endpoints existentes: mismo request, response con campos nuevos

Ningún endpoint existente cambió lo que espera recibir. Todos ganaron campos
nuevos en la respuesta, y algunos campos pasaron a poder venir en `null`.

### `POST /api/v1/core/order-deliveries` (crear orden — flujo corporativo)

Respuesta gana `orderType` (siempre `"Corporate"` en este endpoint, ya que el
esporádico usa el endpoint nuevo de la sección 2):

```jsonc
{
  "id": "...",
  "supplierId": "...",       // antes: siempre string. Ahora: string | null (en este endpoint nunca es null, pero el tipo cambió a nullable)
  "userId": "...",
  "orderType": "Corporate",  // NUEVO
  "department": "SantaCruz",
  "clientPhone": "...", "clientFullName": "...", "clientAddress": "...",
  "deliveryType": "Prepaid",
  "totalPrice": 120.5,
  "details": [
    { "id": "...", "articleId": "...", "articleName": "...", "quantity": 2, "unitPrice": 10, "lineTotal": 20 }
  ]
}
```

### `PUT /api/v1/core/order-deliveries/{id}` (editar orden — solo corporativas)

Mismo cambio de tipo en `supplierId` (nullable) y en `details[].articleId`
(nullable). **Nuevo posible error**: si se intenta editar una orden esporádica
por este endpoint, el backend responde `400` con
`errorKey: "orderdelivery.update.notsupported"`. Como el front de usuarioEmpresa
solo edita órdenes propias (siempre corporativas), esto no debería dispararse en
la práctica, pero conviene manejarlo igual que los demás errores genéricos.

### `GET /api/v1/core/order-deliveries/{id}`

```jsonc
{
  "id": "...",
  "supplierId": "...",       // string | null — NULL si es una orden esporádica
  "supplierName": "...",     // string | null — NULL si es una orden esporádica
  "userId": "...", "userName": "...",
  "orderType": "Corporate",  // NUEVO — "Corporate" | "Sporadic"
  "department": "...", "clientPhone": "...", "clientFullName": "...", "clientAddress": "...",
  "deliveryType": "...",
  "totalPrice": 0, "isAttended": true, "createdAt": "...",
  "details": [
    { "id": "...", "articleId": "...", "articleName": "...", "quantity": 0, "unitPrice": 0, "lineTotal": 0 }
    // articleId ahora es string | null — NULL en líneas de orden esporádica
  ]
}
```

### `GET /api/v1/core/order-deliveries` (paginado)

Mismos agregados que arriba en cada item: `orderType` (nuevo),
`supplierId`/`supplierName` ahora `string | null`.

> **Importante**: cualquier vista que hoy asuma "toda orden tiene proveedor"
> (ej. mostrar `supplierName` sin chequeo, o linkear a la ficha del proveedor) va
> a romper o mostrar `null`/vacío en cuanto existan órdenes esporádicas. Agregar
> chequeo condicional (`supplierName ?? "Cliente esporádico"` o similar) en
> listados y detalle de orden.

### `POST /api/v1/core/shipments`, `PUT /api/v1/core/shipments/{id}`, `GET /api/v1/core/shipments/{id}`, `GET /api/v1/core/shipments` (paginado)

Los cuatro ganan el campo **`code`** (string): el código de guía nuevo con
prefijo (`COR-PAG-000123`, `COR-CXC-000045`, etc). El campo viejo
`waybillNumber` (solo el número, formato `00000123`) **se mantiene sin
cambios**, no se eliminó.

```jsonc
{
  "id": "...", "orderDeliveryId": "...",
  "waybillNumber": "00000123",  // sigue igual, sin prefijo
  "code": "COR-PAG-000123",     // NUEVO — este es el código a mostrar al cliente/repartidor
  "totalWeight": 0, "shippingPrice": 0,
  "details": [ /* ... */ ]
}
```

**Recomendación**: donde hoy se muestre `waybillNumber` al usuario final (ticket,
guía impresa, buscador de envío), migrar a mostrar `code` en su lugar. Es el
identificador legible que el negocio quiere usar de ahora en más; `waybillNumber`
queda como dato interno/legacy.

## 2. Endpoint nuevo: flujo esporádico (orden + guía en un solo paso)

```
POST /api/v1/core/shipments/sporadic
```

Lo usa **solo la vista de admin** (mostrador). No requiere `supplierId` ni
artículos de inventario — nombre del artículo, cantidad, precio, peso y costo de
envío se cargan todos ahí mismo, en la misma llamada.

### Request

```jsonc
{
  "userId": "guid-del-admin-logueado",
  "department": "SantaCruz",   // enum string: Beni, Chuquisaca, Cochabamba, LaPaz, Oruro, Pando, Potosi, SantaCruz, Tarija
  "clientPhone": "70000000",
  "clientFullName": "Juan Pérez",
  "clientAddress": "Av. Siempre Viva 123",
  "deliveryType": "Prepaid",   // "Prepaid" | "CashOnDelivery"
  "lines": [
    {
      "articleName": "Caja de zapatos",
      "quantity": 1,
      "unitPrice": 50,
      "weight": 2.5,
      "shippingCost": 15
    }
  ]
}
```

### Response (201 Created)

```jsonc
{
  "orderDeliveryId": "...",
  "shipmentId": "...",
  "code": "ESP-PAG-000045",
  "totalPrice": 50,
  "totalWeight": 2.5,
  "shippingPrice": 15,
  "details": [
    {
      "orderDeliveryDetailId": "...",
      "shipmentDetailId": "...",
      "articleName": "Caja de zapatos",
      "quantity": 1, "unitPrice": 50, "lineTotal": 50,
      "weight": 2.5, "shippingCost": 15
    }
  ]
}
```

### Errores posibles (400, `errorKey` en el `ProblemDetails`)

| errorKey | Causa |
|---|---|
| `sporadicshipment.lines.required` | No se mandó ninguna línea |
| `sporadicshipment.user.notfound` | El `userId` no existe |
| `sporadicshipment.articlename.required` | Una línea vino sin nombre de artículo |
| `sporadicshipment.quantity.invalid` | `quantity <= 0` en alguna línea |
| `sporadicshipment.weight.invalid` | `weight <= 0` en alguna línea |
| `sporadicshipment.shippingcost.invalid` | `shippingCost < 0` en alguna línea |

> **Sin autorización por rol todavía**: el backend no valida por rol quién puede
> pegarle a este endpoint. Cualquier usuario autenticado, incluido un
> usuarioEmpresa, puede llamarlo hoy. La separación de "quién ve/usa qué botón"
> depende 100% de que el front oculte esta acción fuera de la vista de admin —
> no hay red de seguridad del lado del servidor todavía.

## 3. Checklist para el front

- [ ] Actualizar tipos/interfaces de `OrderDelivery` y `Shipment` con los campos
      nuevos y las nullabilidades marcadas arriba.
- [ ] En listados/detalle de orden: manejar `supplierName`/`supplierId` en
      `null` (órdenes esporádicas).
- [ ] En líneas de detalle de orden: manejar `articleId` en `null` (texto libre,
      sin artículo de inventario detrás).
- [ ] Agregar un badge/columna de `orderType` en el listado de órdenes si se
      quiere distinguir visualmente corporativo vs esporádico.
- [ ] Migrar la visualización del código de guía de `waybillNumber` a `code` en
      pantallas/tickets cara al cliente.
- [ ] Nueva pantalla/flujo de admin que pegue a `POST /shipments/sporadic` con
      un solo formulario (datos del cliente + líneas con
      nombre/cantidad/precio/peso/costo).
- [ ] Ocultar esa pantalla/acción para roles que no sean admin (no hay gate en
      backend por ahora).
