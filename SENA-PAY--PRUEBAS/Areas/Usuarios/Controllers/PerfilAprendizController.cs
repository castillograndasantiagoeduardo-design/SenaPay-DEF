using Microsoft.AspNetCore.Mvc;
using SenaPay.Application.UseCases.Aprendiz;

namespace SENA_PAY__PRUEBAS.Areas.Usuarios.Controllers;

/// <summary>
/// Responsabilidad única: leer sesión, delegar al caso de uso y devolver la vista.
/// Sin lógica de negocio. Sin DbContext.
/// </summary>
[Area("Usuarios")]
public class PerfilAprendizController : Controller
{
    private readonly GetPerfilAprendizUseCase _getPerfilUseCase;

    public PerfilAprendizController(GetPerfilAprendizUseCase getPerfilUseCase)
    {
        _getPerfilUseCase = getPerfilUseCase;
    }

    // GET /Usuarios/PerfilAprendiz/Perfil
    public async Task<IActionResult> Perfil()
    {
        // ── 1. Validar sesión ──────────────────────────────────────────────
        string? doc = HttpContext.Session.GetString("UsuarioDoc");

        if (string.IsNullOrEmpty(doc) || !int.TryParse(doc, out int documento))
            return RedirectToAction("Login", "Account", new { area = "Account" });

        // ── 2. Delegar al Caso de Uso ──────────────────────────────────────
        var perfil = await _getPerfilUseCase.EjecutarAsync(documento);

        if (perfil is null)
            return RedirectToAction("Login", "Account", new { area = "Account" });

        // ── 3. Pasar el DTO completo a la vista (typed model) ─────────────
        return View(perfil);
    }

    // GET /Usuarios/PerfilAprendiz/CerrarSesion
    public IActionResult CerrarSesion()
    {
        HttpContext.Session.Clear();
        return RedirectToAction("Login", "Account", new { area = "Account" });
    }
}