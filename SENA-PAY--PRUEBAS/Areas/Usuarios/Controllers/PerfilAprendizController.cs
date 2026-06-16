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
    private readonly GetPerfilAprendizUseCase        _getPerfilUseCase;
    private readonly ActualizarPerfilAprendizUseCase _actualizarPerfilUseCase;

    public PerfilAprendizController(
        GetPerfilAprendizUseCase        getPerfilUseCase,
        ActualizarPerfilAprendizUseCase actualizarPerfilUseCase)
    {
        _getPerfilUseCase        = getPerfilUseCase;
        _actualizarPerfilUseCase = actualizarPerfilUseCase;
    }

    // GET /Usuarios/PerfilAprendiz/Perfil
    public async Task<IActionResult> Perfil()
    {
        string? doc = HttpContext.Session.GetString("UsuarioDoc");

        if (string.IsNullOrEmpty(doc) || !int.TryParse(doc, out int documento))
            return RedirectToAction("Login", "Account", new { area = "Account" });

        var perfil = await _getPerfilUseCase.EjecutarAsync(documento);

        if (perfil is null)
            return RedirectToAction("Login", "Account", new { area = "Account" });

        return View(perfil);
    }

    // POST /Usuarios/PerfilAprendiz/ActualizarPerfil
    [HttpPost]
    public async Task<IActionResult> ActualizarPerfil([FromBody] ActualizarPerfilDto dto)
    {
        string? doc = HttpContext.Session.GetString("UsuarioDoc");

        if (string.IsNullOrEmpty(doc) || !int.TryParse(doc, out int documento))
            return Json(new { ok = false, msg = "Sesión expirada. Inicia sesión de nuevo." });

        if (string.IsNullOrWhiteSpace(dto?.Correo) ||
            !System.Text.RegularExpressions.Regex.IsMatch(
                dto.Correo, @"^[^\s@]+@[^\s@]+\.[^\s@]+$"))
            return Json(new { ok = false, msg = "El correo no es válido." });

        bool actualizado = await _actualizarPerfilUseCase.EjecutarAsync(documento, dto);

        return actualizado
            ? Json(new { ok = true,  msg = "Perfil actualizado correctamente." })
            : Json(new { ok = false, msg = "No se encontró el aprendiz." });
    }

    // GET /Usuarios/PerfilAprendiz/CerrarSesion
    public IActionResult CerrarSesion()
    {
        HttpContext.Session.Clear();
        return RedirectToAction("Login", "Account", new { area = "Account" });
    }
}
