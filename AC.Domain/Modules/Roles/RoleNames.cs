namespace AC.Domain.Modules.Roles;

/// <summary>
/// Nombres de los roles del sistema tal como se guardan en la tabla roles
/// y viajan en el claim de rol del JWT. Las combinaciones son const para
/// poder usarse en [Authorize(Roles = ...)].
/// </summary>
public static class RoleNames
{
    public const string SuperAdmin     = "superadmin";
    public const string Admin          = "admin";
    public const string UsuarioEmpresa = "usuarioempresa";
    public const string Conductor      = "conductor";

    public const string SuperAdminAdmin               = SuperAdmin + "," + Admin;
    public const string SuperAdminUsuarioEmpresa      = SuperAdmin + "," + UsuarioEmpresa;
    public const string SuperAdminAdminUsuarioEmpresa = SuperAdmin + "," + Admin + "," + UsuarioEmpresa;
    public const string SuperAdminAdminConductor      = SuperAdmin + "," + Admin + "," + Conductor;
    public const string Todos                         = SuperAdmin + "," + Admin + "," + UsuarioEmpresa + "," + Conductor;
}
