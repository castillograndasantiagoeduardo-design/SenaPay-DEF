using System.Security.Claims;

namespace SENA_PAY__PRUEBAS.Middleware;

/// <summary>
/// Capa de defensa extra: intercepta peticiones HTTP y redirige si el usuario
/// autenticado intenta acceder a un área que no le corresponde por rol.
/// Los atributos [Authorize(Roles=X)] en los controladores también lo bloquean,
/// pero este middleware actúa antes y da un mensaje más claro.
/// </summary>
public class RoleGuardMiddleware
{
    private readonly RequestDelegate _next;

    // Prefijos de ruta → roles permitidos (IdRol como string en el claim)
    private static readonly Dictionary<string, string[]> _areaRoles = new(StringComparer.OrdinalIgnoreCase)
    {
        { "/Usuarios/",       new[] { "1" } },           // Solo Aprendiz
        { "/Funcionarios/",   new[] { "2" } },           // Solo Funcionario
        { "/AdminCafeteria/", new[] { "3" } },           // Solo Admin de tienda
        { "/Tienda/",         new[] { "1", "2" } },      // Aprendiz y Funcionario
        { "/Tienda",          new[] { "1", "2" } },      // ruta raíz /Tienda
    };

    public RoleGuardMiddleware(RequestDelegate next) => _next = next;

    public async Task InvokeAsync(HttpContext context)
    {
        var path = context.Request.Path.Value ?? string.Empty;

        foreach (var (prefix, rolesPermitidos) in _areaRoles)
        {
            if (!path.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
                continue;

            // No autenticado → al login
            if (context.User.Identity?.IsAuthenticated != true)
            {
                context.Response.Redirect("/Account/Account/Login");
                return;
            }

            // Autenticado pero con rol incorrecto → página de acceso denegado
            var rolUsuario = context.User.FindFirstValue(ClaimTypes.Role);
            if (!rolesPermitidos.Contains(rolUsuario))
            {
                context.Response.Redirect("/Account/Account/AccesoDenegado");
                return;
            }

            break;
        }

        await _next(context);
    }
}
