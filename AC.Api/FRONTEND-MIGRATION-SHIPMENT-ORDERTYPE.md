# Roles, permisos y alcance por sucursal/departamento

Documentación para el frontend. Cambios de esta versión:

1. **Base de datos reseteada**, migraciones consolidadas en una sola y nuevos
   **datos semilla** (roles, sucursales, proveedores y usuarios).
2. **Autorización por rol** en todos los endpoints: `superadmin`, `admin`,
   `usuarioempresa` y `conductor` (rol nuevo). Rol no permitido ⇒ **403**.
3. **Origen automático**: la orden de entrega ya no recibe el departamento de
   origen desde el front — se toma del usuario que la crea. El envío toma la
   sucursal de origen del admin que atiende.
4. **Alcance automático**: cada rol ve solo lo suyo. El `usuarioempresa` ve los
   datos de su proveedor; el `admin` y el `conductor`, lo de su sucursal /
   departamento.

> ⚠️ **La base de datos fue vaciada.** Todos los IDs anteriores dejaron de
> existir y los tokens viejos apuntan a usuarios inexistentes: hay que
> **volver a loguearse** con las credenciales semilla de abajo.

---

## 1. Datos semilla

### Roles (`roles`)

| Id | Nombre |
|---|---|
| `11111111-1111-1111-1111-111111111111` | `superadmin` |
| `22222222-2222-2222-2222-222222222222` | `admin` |
| `33333333-3333-3333-3333-333333333333` | `usuarioempresa` |
| `44444444-4444-4444-4444-444444444444` | `conductor` |

### Sucursales (`branch_offices`)

| Id | Code | Ciudad | Departamento |
|---|---|---|---|
| `b1111111-1111-1111-1111-111111111111` | `SCZ-01` | Santa Cruz | `SantaCruz` |
| `b2222222-2222-2222-2222-222222222222` | `LPZ-01` | La Paz | `LaPaz` |
| `b3333333-3333-3333-3333-333333333333` | `EAL-01` | El Alto | `LaPaz` |
| `b4444444-4444-4444-4444-444444444444` | `TDD-01` | Trinidad | `Beni` |
| `b5555555-5555-5555-5555-555555555555` | `GYA-01` | Guayaramerín | `Beni` |

### Proveedores (`suppliers`)

| Id | Nombre | Departamento |
|---|---|---|
| `a1111111-1111-1111-1111-111111111111` | Laminas | `SantaCruz` |
| `a2222222-2222-2222-2222-222222222222` | Viralshop | `SantaCruz` |

### Usuarios — password de todos: `Harold123`

Emails guardados en minúsculas. Los GUID repiten el dígito
(`c1111111-1111-1111-1111-111111111111`, etc.).

| Id | Email | Rol | Proveedor | Sucursal |
|---|---|---|---|---|
| `c1111111-…` | harold@gmail.com | superadmin | — | Santa Cruz |
| `c2222222-…` | damian@gmail.com | usuarioempresa | Laminas | — |
| `c3333333-…` | ruben@gmail.com | usuarioempresa | Viralshop | — |
| `c4444444-…` | camila@gmail.com | admin | — | Santa Cruz |
| `c5555555-…` | camilo@gmail.com | admin | — | La Paz |
| `c6666666-…` | wilson@gmail.com | conductor | — | Santa Cruz |
| `c7777777-…` | rolando@gmail.com | conductor | — | La Paz |
| `c8888888-…` | jhonatan@gmail.com | conductor | — | La Paz |

El login (`POST api/v1/core/auth/login`) no cambió: devuelve `role`,
`supplierId`, `supplierName`, `branchOfficeId`, `branchOfficeCode`,
`branchOfficeCity` y `token`. **Guardá `branchOfficeId`**: se usa para los
filtros de envíos entrantes/salientes de la sección 4.

---

## 2. Matriz de permisos

Roles permitidos por endpoint. Cualquier otro rol recibe **403 sin body**
(lo corta el middleware antes del handler).

| Endpoint | superadmin | admin | usuarioempresa | conductor |
|---|---|---|---|---|
| `POST /auth/login` | ✓ anónimo | ✓ | ✓ | ✓ |
| Roles (CRUD) | ✓ | ✗ | ✗ | ✗ |
| Suppliers (CRUD) | ✓ | ✗ | ✗ | ✗ |
| BranchOffices GET | ✓ | ✓ | ✗ | ✗ |
| BranchOffices POST/PUT/DELETE | ✓ | ✗ | ✗ | ✗ |
| Users (CRUD) | ✓ | ✓ solo conductores de su sucursal | ✗ | ✗ |
| Articles GET | ✓ | ✓ | ✓ los suyos | ✗ |
| Articles POST/PUT/DELETE | ✓ | ✓ | ✗ | ✗ |
| ArticleReceipts GET | ✓ | ✓ | ✓ las suyas | ✗ |
| ArticleReceipts POST/PUT/DELETE | ✓ | ✓ | ✗ | ✗ |
| OrderDeliveries GET | ✓ | ✓ las de su departamento | ✓ las suyas | ✗ |
| OrderDeliveries POST/PUT/DELETE | ✓ | ✗ | ✓ las suyas | ✗ |
| Shipments GET | ✓ | ✓ los de su sucursal | ✓ los suyos | ✓ los de su sucursal |
| Shipments POST / sporadic / PUT / DELETE | ✓ | ✓ los de su sucursal | ✗ | ✗ |
| Shipments PATCH `/{id}/status` | ✓ | ✓ los de su sucursal | ✗ | ✓ los de su sucursal |

---

## 3. Origen automático (⚠️ cambia el body de 2 endpoints)

El backend relee al usuario autenticado de la base de datos en cada request y
completa el origen solo. **El front ya no manda datos de origen.**

### 3.1 `POST /order-deliveries` — el `originDepartment` ya no se manda

Nunca se mandó en el body, pero antes se copiaba del proveedor. Ahora la regla es:

- si el usuario que crea la orden **tiene sucursal** → el departamento de esa sucursal;
- si no tiene (caso del `usuarioempresa`) → el departamento de su proveedor.

Ese `originDepartment` es lo que decide **qué admins ven la orden para
atenderla**. Ejemplo: damian (Viralshop, Santa Cruz) crea una orden con destino
La Paz ⇒ la orden queda con `originDepartment: "SantaCruz"` y aparece en la
bandeja de los admins de Santa Cruz, no en la de los de La Paz.

```jsonc
// POST /api/v1/core/order-deliveries   (como damian, usuarioempresa de Laminas)
{
  "destinationDepartment": "LaPaz",     // solo el destino se manda
  "clientPhone": "70011122",
  "clientFullName": "Cliente Ejemplo",
  "clientAddress": "Av. Siempreviva 123",
  "deliveryType": "Prepaid",
  "isExpress": false,
  "lines": [{ "articleId": "…", "quantity": 2, "unitPrice": 25 }]
}
// → 201
{
  "id": "…",
  "originDepartment": "SantaCruz",      // NUEVO: lo puso el backend
  "destinationDepartment": "LaPaz",
  "…": "…"
}
```

### 3.2 `POST /shipments/sporadic` — **quitar `originDepartment` del body**

Este endpoint sí lo recibía y **ya no existe en el request**: se toma del
departamento de la sucursal del usuario que registra el envío esporádico.
Si el front lo sigue mandando, el campo se ignora.

```jsonc
{
  // "originDepartment": "LaPaz",     ← ELIMINAR, lo pone el backend
  "destinationBranchOfficeId": "b1111111-1111-1111-1111-111111111111",
  "senderFullName": "Juan Emisor",
  "senderPhone": "70000000",
  "senderAddress": "Calle 1",
  "destinationDepartment": "SantaCruz",
  "clientPhone": "70000001",
  "clientFullName": "Cliente Esporádico",
  "clientAddress": "Calle 2",
  "deliveryType": "CashOnDelivery",
  "isExpress": false,
  "packageCount": 1,
  "packageDescription": "Sobre",
  "lines": [{ "articleName": "Documento", "quantity": 1, "unitPrice": 10, "weight": 1, "shippingCost": 5 }]
}
```

### 3.3 `POST /shipments` — la sucursal de origen ya salía sola

Se mantiene: el body manda solo `destinationBranchOfficeId`; el origen es la
sucursal del admin que atiende y el envío nace en estado `InTransit`.
**Nuevo**: un admin solo puede atender órdenes cuyo `originDepartment` coincida
con el departamento de su sucursal, si no recibe **403**
`shipment.orderdelivery.forbidden`.

---

## 4. Qué ve cada rol (alcance automático)

Los filtros que el front mandaba para esto se ignoran o se fuerzan server-side.

### usuarioempresa
En `GET /articles`, `/article-receipts`, `/order-deliveries` y `/shipments` el
backend **fuerza `supplierId` = el de su proveedor**. Si manda otro, se ignora.
En los `GET /{id}`, `PUT` y `DELETE`, si el recurso es de otro proveedor ⇒ 403
`*.access.forbidden`.

### admin
- `GET /order-deliveries`: solo las de **su departamento** (su bandeja de
  órdenes por atender). Combinar con `?unattended=true` para las pendientes.
  Una orden de otro departamento devuelve 403 en `GET /{id}`.
- `GET /shipments`: solo aquellos donde **su sucursal es origen o destino**.

### conductor
Solo `GET /shipments`, `GET /shipments/{id}` y `PATCH /shipments/{id}/status`,
siempre limitado a envíos de su sucursal (origen o destino).

### Envíos por despachar vs. envíos por recibir

El listado de envíos ya viene acotado a la sucursal del usuario. Para separar
las dos bandejas, mandá **tu propio `branchOfficeId`** (el del login) en el
filtro correspondiente:

```
GET /api/v1/core/shipments?originBranchOfficeId={miSucursal}        → los que yo despacho
GET /api/v1/core/shipments?destinationBranchOfficeId={miSucursal}   → los que recibo y debo entregar
```

Ambos filtros se pueden combinar con `status` (por ejemplo
`&status=InTransit`). Sin filtros, la lista trae ambas direcciones juntas.

---

## 5. Conductor: cambio de estado

Body del `PATCH /shipments/{id}/status`: se manda `status` **o** `observation`
(nunca ambos) y opcionalmente `deliveryComment`.

Transiciones válidas de `status`:

| Desde | Hacia |
|---|---|
| `Pending` | `InTransit` |
| `InTransit` | `Observed`, `Delivered`, `Rejected`, `Returned` |
| `Observed` | `InTransit`, `Delivered`, `Rejected`, `Returned` |
| `Rejected` | `Returned` |
| `Delivered` / `Returned` | finales |

Observaciones (solo si el envío está `InTransit` u `Observed`); el estado se deriva:

| `observation` | Estado resultante |
|---|---|
| `CustomerRefused` | `Rejected` |
| `NoAnswerDay1`, `NoAnswerDay2`, `NoAnswerDay3`, `CustomerTraveling`, `WrongPhoneNumber`, `TooFar`, `NotDeliveredOnTime`, `InProvince` | `Observed` |

```jsonc
// PATCH /api/v1/core/shipments/{id}/status
{ "observation": "NoAnswerDay1", "deliveryComment": "No contesta" }   // → 200, status Observed
{ "status": "Delivered" }                                             // → 200
// envío de otra sucursal → 403 { "title": "shipment.statuschange.forbidden" }
```

---

## 6. Gestión de conductores por el admin

- `GET /users` como admin ⇒ solo conductores de su sucursal (los filtros
  `role` y `supplierId` del request se ignoran).
- `POST /users` como admin ⇒ el `roleId` debe ser el de conductor y el
  `branchOfficeId` su propia sucursal; si no, 403 (`user.role.forbidden` /
  `user.branchoffice.forbidden`).
- `PUT` / `DELETE /users/{id}` como admin ⇒ solo si el usuario objetivo es
  conductor de su sucursal (403 `user.access.forbidden`); no puede cambiarle el
  rol ni moverlo de sucursal.

Coherencia rol ↔ alcance, válida para **cualquier** actor (también superadmin):

- rol `usuarioempresa` ⇒ `supplierId` obligatorio y `branchOfficeId` en `null`;
- rol `admin` o `conductor` ⇒ `branchOfficeId` obligatorio y `supplierId` en `null`.

---

## 7. Error keys

Todos los errores de negocio llegan como `ProblemDetails` con `title` = error
key y `detail` = mensaje en español. (El 403 por rol no permitido en el
endpoint viene **sin body**.)

| Error key | HTTP | Cuándo |
|---|---|---|
| `article.access.forbidden`, `articlereceipt.access.forbidden`, `orderdelivery.access.forbidden`, `shipment.access.forbidden`, `user.access.forbidden` | 403 | El recurso no es del proveedor / sucursal / departamento del usuario |
| `shipment.orderdelivery.forbidden` | 403 | El admin intenta atender una orden de otro departamento |
| `shipment.statuschange.forbidden` | 403 | Cambio de estado sobre un envío de otra sucursal |
| `user.role.forbidden` | 403 | El admin intenta crear/asignar un rol distinto de conductor |
| `user.branchoffice.forbidden` | 403 | El admin intenta crear/mover un conductor a otra sucursal |
| `article.user.notsupplier`, `articlereceipt.user.notsupplier`, `orderdelivery.user.notsupplier`, `shipment.user.notsupplier` | 400 | El usuarioempresa no tiene proveedor asignado |
| `orderdelivery.user.nobranch`, `shipment.user.nobranch`, `user.actor.nobranch` | 400 | El admin/conductor no tiene sucursal asignada |
| `shipment.originbranch.missing` | 400 | Quien atiende no tiene sucursal (no se puede calcular el origen) |
| `user.supplierid.required` / `user.supplierid.notallowed` | 400 | Coherencia rol ↔ proveedor |
| `user.branchofficeid.required` / `user.branchofficeid.notallowed` | 400 | Coherencia rol ↔ sucursal |
| `user.actor.notfound`, `*.user.notfound` | 404 | El usuario del token ya no existe |

Siguen vigentes los previos: `shipment.statuschange.invalidtransition`,
`shipment.observation.invalidstatus`, `shipment.alreadyattended`,
`orderdelivery.alreadyattended`, `orderdelivery.stock.insufficient`, etc.

> Recordá que el **stock** de un artículo entra solo por recepciones:
> `POST /articles` lo crea con `count` 0 y sube con `POST /article-receipts`.

---

## 8. Acción para el front

- [ ] Actualizar credenciales de desarrollo a los usuarios semilla
      (password `Harold123`) y forzar re-login: tokens e IDs viejos no sirven.
- [ ] **Quitar `originDepartment`** del body de `POST /shipments/sporadic` y
      del formulario de envío esporádico (ya no se elige).
- [ ] Quitar cualquier selector de departamento de origen en la creación de
      órdenes: el backend lo resuelve y lo devuelve en la respuesta.
- [ ] Guardar `role` y `branchOfficeId` del login; ocultar menús según la
      matriz de la sección 2.
- [ ] Como `usuarioempresa`, dejar de mandar `supplierId` en los listados.
- [ ] Pantalla del admin: bandeja de **órdenes por atender**
      (`/order-deliveries?unattended=true`, ya filtrada por su departamento) y
      dos listas de envíos, **por despachar** (`originBranchOfficeId=miSucursal`)
      y **por entregar** (`destinationBranchOfficeId=miSucursal`).
- [ ] Manejar 403 con `ProblemDetails` (keys `*.forbidden`) y el 403 sin body
      del middleware de roles.
- [ ] Pantalla de conductores del admin: rol conductor fijo y su sucursal
      preseleccionada/bloqueada.
- [ ] Si el superadmin cambia rol/proveedor/sucursal de un usuario, ese usuario
      debe re-loguearse para que el token refleje el cambio.
