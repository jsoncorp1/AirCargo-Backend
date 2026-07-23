# Guía de migración para el frontend — Autenticación obligatoria (JWT)

**Esto rompe todo el front si no se manda el token.** A partir de ahora,
**todo endpoint de la API (menos login) exige un `Authorization: Bearer <token>`
válido**, o responde `401 Unauthorized` sin ni siquiera ejecutar la lógica.

---

## 1. Qué cambió

Antes, el JWT se emitía en el login pero **ningún endpoint lo exigía** — se
podía llamar a cualquier endpoint sin token, y el backend confiaba en lo que
mandara el body (por ejemplo, `userId` se pasaba como campo del request en
vez de sacarse del token). Eso ya no es así:

- Se agregó una política de autorización global: **todo endpoint requiere un
  JWT válido por defecto**. Solo `POST /api/v1/core/auth/login` queda público.
- Si falta el header `Authorization`, o el token es inválido/expiró, el
  backend responde `401 Unauthorized` (sin `ProblemDetails` con `errorKey`,
  es el `401` estándar de ASP.NET Core).

## 2. `userId` ya NO se manda en el body (rompe contrato)

En los dos endpoints que crean una orden/guía, el campo `userId` **se quitó
del request** — el backend ya no confía en lo que mande el front, lo saca
del token (`sub`) del usuario logueado.

### `POST /api/v1/core/order-deliveries`

```jsonc
// ANTES
{
  "userId": "guid-del-usuario",   // SE QUITÓ — ya no va en el body
  "destinationDepartment": "LaPaz",
  "clientPhone": "...", "clientFullName": "...", "clientAddress": "...",
  "deliveryType": "Prepaid",
  "lines": [ /* ... */ ]
}

// AHORA
{
  "destinationDepartment": "LaPaz",
  "clientPhone": "...", "clientFullName": "...", "clientAddress": "...",
  "deliveryType": "Prepaid",
  "lines": [ /* ... */ ]
}
```

### `POST /api/v1/core/shipments/sporadic`

Mismo caso — se quita `userId` del body, todo lo demás queda igual:

```jsonc
// ANTES: tenía "userId": "guid-del-admin-logueado" como primer campo
// AHORA: ese campo ya no existe, el resto del body no cambia
{
  "originDepartment": "...", "senderFullName": "...", "senderPhone": "...", "senderAddress": "...",
  "destinationDepartment": "...", "clientPhone": "...", "clientFullName": "...", "clientAddress": "...",
  "deliveryType": "Prepaid",
  "packageCount": 1, "packageDescription": "1 caja",
  "lines": [ /* ... */ ]
}
```

Si el front sigue mandando `userId` en el body de estos dos, no pasa nada
(el backend lo ignora), pero ya no hace falta armarlo ni enviarlo.

## 3. Acción para el front

- [ ] **Mandar `Authorization: Bearer <token>` en absolutamente todas las
      llamadas a `api/v1/core/*`**, excepto `POST /auth/login`. Si hay algún
      cliente HTTP centralizado (axios/fetch wrapper), es el lugar para
      agregar el header una sola vez en vez de endpoint por endpoint.
- [ ] Manejar `401` globalmente (interceptor de axios/fetch): si cualquier
      llamada devuelve `401`, lo más probable es que no falta el token o el
      token expiró — redirigir a login / refrescar sesión.
- [ ] Quitar `userId` del body en `POST /order-deliveries` y
      `POST /shipments/sporadic` (ya no lo pide el backend; mandarlo no rompe
      nada, pero está de más).
- [ ] Revisar que el token que se guarda tras el login efectivamente se esté
      adjuntando en **todas** las pantallas, no solo en las que se probaron
      hasta ahora — este cambio afecta a Users, Suppliers, Articles,
      ArticleReceipts, Roles, OrderDeliveries y Shipments por igual, no solo
      a lo de envíos.
