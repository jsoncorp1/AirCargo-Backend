# Multisucursal: sucursales, usuarios por sucursal y estados de envío

Documentación para el frontend de los cambios del modelo multisucursal:

1. Nueva entidad **BranchOffice** (sucursales) con su CRUD.
2. Los **usuarios** ahora pueden pertenecer a una sucursal (relación nullable).
3. Los **shipments** registran sucursal de **origen** y **destino**, tienen
   **estado**, **observación** del delivery y **comentario** del delivery.
4. Nuevos **filtros** en el listado de shipments (proveedor, sucursales, estado).

---

## 1. Sucursales (`BranchOffice`)

Entidad nueva. Ruta base: `api/v1/core/branch-offices`.

| Campo | Tipo | Notas |
|---|---|---|
| `id` | guid | |
| `code` | string (≤20) | Único entre sucursales activas |
| `bolivianDepartment` | enum string | `LaPaz`, `SantaCruz`, `Cochabamba`, etc. |
| `city` | string (≤100) | |
| `address` | string? (≤300) | Opcional |
| `latitude` / `longitude` | double? | Opcionales |
| `phone` | string (≤30) | |
| `active` | bool | Soft delete |

### Endpoints

- `POST /api/v1/core/branch-offices` — crea una sucursal.
- `PUT /api/v1/core/branch-offices/{id}` — actualiza.
- `DELETE /api/v1/core/branch-offices/{id}` — soft delete.
- `GET /api/v1/core/branch-offices/{id}` — detalle.
- `GET /api/v1/core/branch-offices?page=1&perPage=10` — listado paginado.

```jsonc
// POST /api/v1/core/branch-offices
{
  "code": "SCZ-01",
  "bolivianDepartment": "SantaCruz",
  "city": "Santa Cruz de la Sierra",
  "address": "Av. Ejemplo 123",     // opcional
  "latitude": -17.7833,             // opcional
  "longitude": -63.1821,            // opcional
  "phone": "77712345"
}
```

---

## 2. Usuarios: sucursal opcional (`branchOfficeId`)

Un usuario (por ejemplo con rol admin o de mostrador) puede pertenecer a una
sucursal. Es **nullable**: los usuarios de proveedores no llevan sucursal.

**Se manda (request) en:**
- `POST /api/v1/core/users` — nuevo campo `branchOfficeId` (guid, opcional).
- `PUT /api/v1/core/users/{id}` — ídem.

**Se devuelve (response) en:**
- Respuestas de los endpoints de arriba (`branchOfficeId`).
- `GET /api/v1/core/users/{id}` y `GET /api/v1/core/users` (paginado):
  `branchOfficeId`, `branchOfficeCode`, `branchOfficeCity`.

> **Importante:** la sucursal del usuario autenticado es la que se usa como
> **origen** al atender un envío. Un usuario sin sucursal asignada **no puede
> atender envíos** (el backend responde `shipment.originbranch.missing`).

### Login: sucursal en la respuesta y en el token

`POST /api/v1/core/auth/login` ahora devuelve también `branchOfficeId`,
`branchOfficeCode` y `branchOfficeCity` (los tres `null` si el usuario no
tiene sucursal).

Además, el **JWT incluye el claim `branchOfficeId`** junto a `supplierId`
(string vacío si el usuario no tiene sucursal). El front puede leerlo del
token para mostrar la sucursal activa sin llamadas extra.

```jsonc
// Payload del JWT (decodificado)
{
  "sub": "…",
  "email": "…",
  "role": "Admin",
  "supplierId": "",
  "branchOfficeId": "3f2a…",   // NUEVO — "" si no tiene sucursal
  "jti": "…"
}
```

> Ojo: si se cambia la sucursal de un usuario, el claim queda desactualizado
> hasta que vuelva a loguearse. El backend no confía en el claim para atender
> envíos: siempre valida la sucursal actual del usuario en la base de datos.

### Acción para el front

- [ ] Agregar un select de sucursal (opcional) en los formularios de crear y
      editar usuario (llenar con `GET /branch-offices`).
- [ ] Tomar `branchOfficeId`/`branchOfficeCode`/`branchOfficeCity` de la
      respuesta del login (o el claim `branchOfficeId` del token) para
      mostrar la sucursal activa del usuario.

---

## 3. Shipments multisucursal, estados y observaciones

### Sucursales de origen y destino

Al atender una orden (crear el shipment) se registran:

- **Origen** (`originBranchOfficeId`): lo pone el backend automáticamente con
  la sucursal del usuario autenticado — **no se manda en el request**.
- **Destino** (`destinationBranchOfficeId`): **se manda en el request** (nuevo
  campo obligatorio).

Aplica a:
- `POST /api/v1/core/shipments` (atender una orden corporativa)
- `POST /api/v1/core/shipments/sporadic` (envío esporádico)

```jsonc
// POST /api/v1/core/shipments
{
  "orderDeliveryId": "…",
  "destinationBranchOfficeId": "…",   // NUEVO — obligatorio
  "packageCount": 3,
  "packageDescription": "3 cajas",
  "lines": [ /* igual que antes */ ]
}
```

En los shipments creados antes de este cambio, ambas sucursales vienen `null`.

### Estado (`status`)

Enum string. Un shipment **nace en `InTransit`** al ser atendido.

| Valor | Significado |
|---|---|
| `Pending` | Pendiente |
| `InTransit` | En tránsito (estado inicial) |
| `Observed` | Observado (se llega al registrar una observación) |
| `Delivered` | Entregado |
| `Rejected` | Rechazado |
| `Returned` | Devuelto |

Transiciones válidas para cambio manual de estado:

- `Pending` → `InTransit`
- `InTransit` → `Observed`, `Delivered`, `Rejected`, `Returned`
- `Observed` → `InTransit`, `Delivered`, `Rejected`, `Returned`
- `Rejected` → `Returned`
- `Delivered` y `Returned` son finales.

### Observación (`observation`)

Enum string, nullable. Al registrar una observación **el estado cambia solo**:

- `CustomerRefused` (no quiere) → el estado pasa a **`Rejected`**.
- Cualquier otra → el estado pasa a **`Observed`**.

| Valor | Significado |
|---|---|
| `CustomerRefused` | No quiere |
| `NoAnswerDay1` | No contesta día 1 |
| `NoAnswerDay2` | No contesta día 2 |
| `NoAnswerDay3` | No contesta día 3 |
| `CustomerTraveling` | Está de viaje |
| `WrongPhoneNumber` | Número incorrecto |
| `TooFar` | Muy lejos |
| `NotDeliveredOnTime` | No se entregó a tiempo |
| `InProvince` | En provincia |

Solo se puede observar un envío en estado `InTransit` u `Observed`.

### Comentario del delivery (`deliveryComment`)

Texto libre, nullable, ≤500 caracteres.

### Nuevo endpoint: cambiar estado / observar / comentar

`PATCH /api/v1/core/shipments/{id}/status`

Mandar **`status` o `observation`, no ambos** (la observación define el estado
sola). `deliveryComment` puede ir solo o acompañando a cualquiera de los dos.

```jsonc
// Cambio manual de estado
{ "status": "Delivered" }

// Registrar observación (el estado pasa a Observed)
{ "observation": "NoAnswerDay1", "deliveryComment": "Llamé 3 veces" }

// Observación de rechazo (el estado pasa a Rejected)
{ "observation": "CustomerRefused" }
```

Respuesta: `{ id, code, status, observation, deliveryComment }`.

Errores (`400`, con `title` = clave):
- `shipment.statuschange.empty` — no se mandó nada.
- `shipment.statuschange.conflict` — se mandaron `status` y `observation` juntos.
- `shipment.statuschange.invalidtransition` — transición de estado no permitida.
- `shipment.observation.invalidstatus` — el envío no está en `InTransit`/`Observed`.

### Listado de shipments: nuevos filtros y campos

`GET /api/v1/core/shipments` — filtros opcionales y **combinables** (sin
filtros se ve todo en general):

| Query param | Filtra por |
|---|---|
| `supplierId` | Proveedor de la orden |
| `originBranchOfficeId` | Sucursal de origen |
| `destinationBranchOfficeId` | Sucursal de destino |
| `status` | Estado (`InTransit`, `Observed`, …) |

Cada ítem del listado ahora incluye además: `supplierId`,
`originBranchOfficeId`, `originBranchOfficeCode`, `destinationBranchOfficeId`,
`destinationBranchOfficeCode`, `status`, `observation`.

`GET /api/v1/core/shipments/{id}` incluye además: sucursales de origen/destino
(id, `code`, `city`), `status`, `observation` y `deliveryComment`.

### Acción para el front

- [ ] En el formulario de atender orden y de envío esporádico: select de
      **sucursal de destino** (obligatorio). El origen no se pide.
- [ ] Vista de shipments: columna de estado (badge) y filtros por proveedor,
      sucursal origen/destino y estado.
- [ ] Pantalla/modal del delivery: registrar observación + comentario, y
      cambio manual de estado respetando las transiciones de arriba.
