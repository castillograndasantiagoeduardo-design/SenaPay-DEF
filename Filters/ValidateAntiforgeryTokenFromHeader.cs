using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace SENA_PAY__PRUEBAS.Filters;

/// <summary>
/// Valida el token antiforgery leyéndolo desde el header HTTP
/// en lugar del cuerpo. Necesario cuando el body es JSON ([FromBody]).
/// </summary>
public class ValidateAntiforgeryTokenFromHeaderAttribute
    : TypeFilterAttribute
{
    public ValidateAntiforgeryTokenFromHeaderAttribute()
        : base(typeof(AntiforgeryFilter)) { }

    private class AntiforgeryFilter : IAsyncAuthorizationFilter
    {
        private readonly IAntiforgery _antiforgery;
        public AntiforgeryFilter(IAntiforgery antiforgery)
            => _antiforgery = antiforgery;

        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            try
            {
                await _antiforgery.ValidateRequestAsync(context.HttpContext);
            }
            catch (AntiforgeryValidationException)
            {
                context.Result = new JsonResult(new { ok = false, msg = "Token de seguridad inválido." })
                {
                    StatusCode = 400
                };
            }
        }
    }
}