using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using AC.Application.Abstractions.Security;
using Microsoft.AspNetCore.Http;

namespace AC.Infrastructure.Services.Security;

public class HttpContextCurrentUser(IHttpContextAccessor httpContextAccessor) : ICurrentUser
{
    private ClaimsPrincipal? Principal => httpContextAccessor.HttpContext?.User;

    public bool IsAuthenticated => Principal?.Identity?.IsAuthenticated ?? false;

    // El handler de JwtBearer puede o no remapear los nombres cortos ("sub",
    // "email") a los ClaimTypes legacy según la versión; se busca en ambos
    // para no depender de ese detalle de configuración.
    public Guid? UserId => ParseGuid(
        Principal?.FindFirst(JwtRegisteredClaimNames.Sub)?.Value
        ?? Principal?.FindFirst(ClaimTypes.NameIdentifier)?.Value);

    public string? Email =>
        Principal?.FindFirst(JwtRegisteredClaimNames.Email)?.Value
        ?? Principal?.FindFirst(ClaimTypes.Email)?.Value;

    public string? Role => Principal?.FindFirst(ClaimTypes.Role)?.Value;

    public Guid? SupplierId => ParseGuid(Principal?.FindFirst("supplierId")?.Value);

    private static Guid? ParseGuid(string? value) =>
        Guid.TryParse(value, out var id) ? id : null;
}
