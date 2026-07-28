# Roles, permisos, scoping por rol y datos semilla

Documentación para el frontend de los cambios de esta versión:

1. **Base de datos reseteada** y migraciones consolidadas en una sola; nuevos
   datos semilla (roles, sucursales, proveedores y usuarios).
2. **Autorización por rol** en TODOS los endpoints: superadmin, admin,
   usuarioempresa y conductor (rol nuevo). Un request con rol no permitido
   recibe **403**.
3. **Scoping automático server-side**: usuarioempresa solo ve datos de SU
   proveedor; admin y conductor solo ven envíos de SU sucursal. Los filtros
   que mandaba el front para esto ahora se ignoran o se fuerzan en el backend.

> ⚠️ **La base de datos fue vaciada por completo.** Todos los IDs anteriores
> (artículos, órdenes, envíos, usuarios, sucursales) dejaron de existir. Los
> tokens viejos apuntan a usuarios que ya no existen: **hay que volver a
> loguearse** con las credenciales semilla de abajo.

---

## 1. Datos semilla

### Roles (`roles`)

| Id | Nombre | Descripción |
|---|---|---|
| `11111111-1111-1111-1111-111111111111` | `superadmin` | Acceso total al sistema |
| `22222222-2222-2222-2222-222222222222` | `admin` | Administración general de su sucursal |
| `33333333-3333-3333-3333-333333333333` | `usuarioempresa` | Usuario de empresa proveedora |
| `44444444-4444-4444-4444-444444444444` | `conductor` | Conductor de reparto (**NUEVO**) |

### Sucursales (`branch_offices`)

| Id | Code | Ciudad | Departamento | Teléfono |
|---|---|---|---|---|
| `b1111111-1111-1111-1111-111111111111` | `SCZ-01` | Santa Cruz | `SantaCruz` | 70000001 |
| `b2222222-2222-2222-2222-222222222222` | `LPZ-01` | La Paz | `LaPaz` | 70000002 |
| `b3333333-3333-3333-3333-333333333333` | `EAL-01` | El Alto | `LaPaz` | 70000003 |
| `b4444444-4444-4444-4444-444444444444` | `TDD-01` | Trinidad | `Beni` | 70000004 |
| `b5555555-5555-5555-5555-555555555555` | `GYA-01` | Guayaramerín | `Beni` | 70000005 |

### Proveedores (`suppliers`)

| Id | Nombre | Departamento |
|---|---|---|
| `a1111111-1111-1111-1111-111111111111` | Laminas | `SantaCruz` |
| `a2222222-2222-2222-2222-222222222222` | Viralshop | `SantaCruz` |

### Usuarios (`users`) — password de TODOS: `Harold123`

Los emails se guardan **en minúsculas**; el login es por email exacto.

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

(Los GUID completos repiten el dígito: `c1111111-1111-1111-1111-111111111111`, etc.)

El login (`POST api/v1/core/auth/login`) no cambió: sigue devolviendo `role`,
`supplierId`, `branchOfficeId`, etc., y el JWT lleva esos mismos claims.

---

## 2. Matriz de permisos por endpoint

Roles permitidos por módulo/verbo. Cualquier otro rol recibe **403** (del
middleware, body vacío). "suyos" = scoping automático de la sección 3.

| Endpoint | superadmin | admin | usuarioempresa | conductor |
|---|---|---|---|---|
| `POST /auth/login` | ✓ (anónimo) | ✓ | ✓ | ✓ |
| Roles (CRUD completo) | ✓ | ✗ | ✗ | ✗ |
| Suppliers (CRUD completo) | ✓ | ✗ | ✗ | ✗ |
| BranchOffices GET (lista y por id) | ✓ | ✓ | ✗ | ✗ |
| BranchOffices POST/PUT/DELETE | ✓ | ✗ | ✗ | ✗ |
| Users (CRUD completo) | ✓ | ✓ solo conductores de su sucursal | ✗ | ✗ |
| Articles GET | ✓ | ✓ | ✓ solo los suyos | ✗ |
| Articles POST/PUT/DELETE | ✓ | ✓ | ✗ | ✗ |
| ArticleReceipts GET | ✓ | ✓ | ✓ solo las suyas | ✗ |
| ArticleReceipts POST/PUT/DELETE | ✓ | ✓ | ✗ | ✗ |
| OrderDeliveries GET | ✓ | ✓ | ✓ solo las suyas | ✗ |
| OrderDeliveries POST/PUT/DELETE | ✓ | ✗ | ✓ solo las suyas | ✗ |
| Shipments GET | ✓ | ✓ su sucursal | ✓ los suyos | ✓ su sucursal |
| Shipments POST / POST sporadic / PUT / DELETE | ✓ | ✓ su sucursal | ✗ | ✗ |
| Shipments PATCH `/{id}/status` | ✓ | ✓ su sucursal | ✗ | ✓ su sucursal |

---

## 3. Scoping automático — qué filtros ya NO debe mandar el front

El backend relee al usuario de BD en cada request (no confía en los claims del
JWT) y decide el alcance según su rol:

- **usuarioempresa**: en `GET /articles`, `GET /article-receipts`,
  `GET /order-deliveries` y `GET /shipments` el backend **fuerza**
  `supplierId = el del usuario`. Si el front manda otro `supplierId`, **se
  ignora**. En los `GET /{id}`, PUT y DELETE, si el recurso no es de su
  proveedor responde **403** (`*.access.forbidden`).
- **admin** y **conductor**: `GET /shipments` y `GET /shipments/{id}` se
  limitan a envíos cuyo **origen o destino** sea su sucursal. Lo mismo aplica
  a PUT/DELETE de shipments (admin) y al PATCH de estado (ambos).
- **superadmin**: sin restricciones; todos los filtros del request se respetan.

```jsonc
// GET /api/v1/core/articles?supplierId=<otro-proveedor>   (como damian, usuarioempresa)
// → 200, pero la lista viene SOLO con artículos de Laminas (el filtro se ignoró)

// GET /api/v1/core/articles/{id-de-articulo-de-viralshop}  (como damian)
// → 403
{
  "title": "article.access.forbidden",           // NUEVO
  "detail": "El artículo no pertenece al proveedor del usuario."
}
```

> Los errores de permisos siempre llegan como `ProblemDetails` con `title` =
> error key y `detail` = mensaje en español. El 403 del middleware (rol no
> permitido en el endpoint) llega **sin body**.

---

## 4. Conductor: cambio de estado de envíos

El conductor solo usa `GET /shipments`, `GET /shipments/{id}` y
`PATCH /shipments/{id}/status`, siempre sobre envíos de su sucursal.

Body del PATCH (sin cambios de formato): mandar `status` **o** `observation`
(no ambos), y opcionalmente `deliveryComment`.

Transiciones de `status` válidas:

| Desde | Hacia |
|---|---|
| `Pending` | `InTransit` |
| `InTransit` | `Observed`, `Delivered`, `Rejected`, `Returned` |
| `Observed` | `InTransit`, `Delivered`, `Rejected`, `Returned` |
| `Rejected` | `Returned` |
| `Delivered` / `Returned` | (finales, ninguna) |

Observaciones (solo con envío `InTransit` u `Observed`); el estado se deriva:

| `observation` | Estado resultante |
|---|---|
| `CustomerRefused` | `Rejected` |
| `NoAnswerDay1..3`, `CustomerTraveling`, `WrongPhoneNumber`, `TooFar`, `NotDeliveredOnTime`, `InProvince` | `Observed` |

```jsonc
// PATCH /api/v1/core/shipments/{id}/status   (como wilson, conductor SCZ)
{ "status": "Delivered", "deliveryComment": "Entregado al cliente" }
// → 200

// mismo PATCH sobre un envío La Paz → Trinidad
// → 403 { "title": "shipment.statuschange.forbidden", ... }   // NUEVO
```

---

## 5. Gestión de conductores por admin

El admin puede usar el CRUD de `/users`, pero **solo** sobre conductores de su
propia sucursal:

- `GET /users` como admin devuelve **solo conductores de su sucursal**
  (los filtros `role`/`supplierId` del request se ignoran).
- `POST /users` como admin: el `roleId` debe ser el de conductor y el
  `branchOfficeId` debe ser su sucursal; si no, 403 (`user.role.forbidden` /
  `user.branchoffice.forbidden`).
- `PUT` / `DELETE /users/{id}` como admin: solo si el usuario objetivo es
  conductor de su sucursal (403 `user.access.forbidden`); no puede cambiarle
  el rol ni moverlo de sucursal.

Además, para **cualquier** actor (también superadmin) se valida coherencia
rol ↔ alcance al crear/editar usuarios:

- rol `usuarioempresa` ⇒ `supplierId` obligatorio y `branchOfficeId` en null.
- rol `admin` o `conductor` ⇒ `branchOfficeId` obligatorio y `supplierId` en null.

---

## 6. Error keys nuevos

| Error key | HTTP | Cuándo |
|---|---|---|
| `*.access.forbidden` (`article.`, `articlereceipt.`, `orderdelivery.`, `shipment.`, `user.`) | 403 | El recurso no pertenece al proveedor/sucursal del usuario |
| `shipment.statuschange.forbidden` | 403 | Admin/conductor intenta cambiar estado de un envío de otra sucursal |
| `user.role.forbidden` | 403 | Admin intenta crear/asignar un rol distinto de conductor |
| `user.branchoffice.forbidden` | 403 | Admin intenta crear/mover un conductor a otra sucursal |
| `*.user.notsupplier` (`article.`, `articlereceipt.`, `orderdelivery.`, `shipment.`) | 400 | El usuarioempresa autenticado no tiene proveedor asignado |
| `shipment.user.nobranch`, `user.actor.nobranch` | 400 | El admin/conductor autenticado no tiene sucursal asignada |
| `user.supplierid.required` / `user.supplierid.notallowed` | 400 | Coherencia rol ↔ proveedor al crear/editar usuario |
| `user.branchofficeid.required` / `user.branchofficeid.notallowed` | 400 | Coherencia rol ↔ sucursal al crear/editar usuario |
| `*.user.notfound`, `user.actor.notfound` | 404 | El usuario autenticado ya no existe en BD (token huérfano) |

Se mantienen los existentes (`shipment.statuschange.invalidtransition`,
`shipment.observation.invalidstatus`, `orderdelivery.alreadyattended`, etc.).

---

## 7. Acción para el front

- [ ] Actualizar las credenciales de desarrollo a los usuarios semilla
      (password `Harold123`) y forzar re-login: los tokens e IDs viejos ya no
      sirven.
- [ ] Guardar el `role` del login y **ocultar menús/acciones según la matriz**
      de la sección 2 (el backend igual bloquea, pero la UI no debe ofrecer lo
      prohibido).
- [ ] Como usuarioempresa, **dejar de mandar `supplierId`** en los listados: el
      backend lo fuerza; el selector de proveedor solo tiene sentido para
      superadmin (y admin donde aplique).
- [ ] Como admin/conductor, asumir que los listados de envíos ya vienen
      filtrados por su sucursal.
- [ ] Manejar **403 con body `ProblemDetails`** (error keys `*.forbidden`) y el
      403 **sin body** del middleware de roles.
- [ ] Pantalla de conductores para admin: solo rol conductor y su sucursal
      preseleccionada/bloqueada en el formulario.
- [ ] Si superadmin cambia rol/proveedor/sucursal de un usuario, ese usuario
      debe **re-loguearse** para que su token refleje el cambio.
