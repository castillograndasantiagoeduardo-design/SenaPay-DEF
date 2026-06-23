using Microsoft.AspNetCore.Mvc;
using SenaPay.Application.UseCases.Aprendiz; //Se conecta con la capa de aplicacion Aprendiz

namespace SENA_PAY__PRUEBAS.Areas.Usuarios.Controllers;

/// <summary>
/// Responsabilidad única: recibir la petición HTTP, leer la sesión,
/// delegar al caso de uso y devolver la vista o redirección.
/// Sin lógica de negocio. Sin DbContext ni hablar con la base de datos.
/// Ahora es un controlador limpio y delgado
/// </summary>
[Area("Usuarios")]
public class PerfilAprendizController : Controller
{
    //Se guarda el caso de uso
    private readonly GetPerfilAprendizUseCase _getPerfilUseCase;

    //Constructor del caso de uso
    public PerfilAprendizController(GetPerfilAprendizUseCase getPerfilUseCase)
    {
        _getPerfilUseCase = getPerfilUseCase;
    }

    //Accion que se dispara al momento que el usuario lo decida
    public async Task<IActionResult> Perfil()
    {
        // ── 1. Validar sesión (responsabilidad de la capa de Presentación) ──
        string? doc = HttpContext.Session.GetString("UsuarioDoc");

        //Esto se hace por seguridad de la interfaz, evalua 2 cosas
        //Si el documento esta vacio (IsNullOrEmpty)
        //Si el texto de la sesion no se puede convetir a numero entero
        //Si cumple una de las dos cancela el inicio de sesion y lo redirigue al login 
        if (string.IsNullOrEmpty(doc) || !int.TryParse(doc, out int documento))
            return RedirectToAction("Login", "Account", new { area = "Account" });

        // ── . Delegar al Caso de Uso ────────────────────────────────────────
        //Se le manda el documento al caso de uso para que el caso de uso busque en la BSD y devuelva nulo o el aprendiz
        var perfil = await _getPerfilUseCase.EjecutarAsync(documento);

        //Si retorna null lo redirigue al login
        if (perfil is null)
            return RedirectToAction("Login", "Account", new { area = "Account" });

        // ── . Pasar datos a la Vista (solo mapeo de DTO → ViewBag) ──────────
        ViewBag.Nombre = perfil.Nombre;
        ViewBag.Saldo = perfil.Saldo;
        ViewBag.Ficha = perfil.Ficha;
        ViewBag.Correo = perfil.Correo;

        //Por ultimo retorna a la vista del perfil del aprendiz ya con sus datos pintados en la vista 
        return View();
    }
}