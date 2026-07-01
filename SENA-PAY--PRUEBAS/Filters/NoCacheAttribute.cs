using Microsoft.AspNetCore.Mvc.Filters;

namespace SENA_PAY__PRUEBAS.Filters;

/// <summary>
/// Evita que el navegador cachee páginas protegidas.
/// Impide que el botón "atrás" muestre la página después de cerrar sesión.
/// </summary>
public class NoCacheAttribute : ActionFilterAttribute
{
    public override void OnResultExecuting(ResultExecutingContext context)
    {
        context.HttpContext.Response.Headers["Cache-Control"] = "no-cache, no-store, must-revalidate";
        context.HttpContext.Response.Headers["Pragma"]        = "no-cache";
        context.HttpContext.Response.Headers["Expires"]       = "0";
        base.OnResultExecuting(context);
    }
}
