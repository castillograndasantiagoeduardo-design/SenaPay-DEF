// Areas/Tienda/Controllers/AccesoTiendaController.cs
using Microsoft.AspNetCore.Mvc;
using SenaPay.Application.UseCases.Tienda;
using System.Security.Claims;

namespace SENA_PAY__PRUEBAS.Areas.AdminCafeteria.Controllers;

[Area("AdminCafeteria")]
public class AccesoTiendaController : Controller
{
    private readonly ObtenerTiendasDeAdminUseCase _obtenerTiendasUseCase;
    private readonly SeleccionarTiendaUseCase _seleccionarTiendaUseCase;

    public const string SessionKeyTienda = "TiendaActual_Id";
    public const string SessionKeyTiendaNombre = "TiendaActual_Nombre";

    public AccesoTiendaController(
        ObtenerTiendasDeAdminUseCase obtenerTiendasUseCase,
        SeleccionarTiendaUseCase seleccionarTiendaUseCase)
    {
        _obtenerTiendasUseCase = obtenerTiendasUseCase;
        _seleccionarTiendaUseCase = seleccionarTiendaUseCase;
    }

    // GET /Tienda/AccesoTienda/SeleccionarTienda
    // Se llama justo después del login si el rol es AdminTienda
    [HttpGet]
    [Route("AdminCafeteria/SeleccionarTienda")]
    public async Task<IActionResult> SeleccionarTienda()
    {
        int idUsuario = ObtenerIdUsuario();
        var tiendas = await _obtenerTiendasUseCase.EjecutarAsync(idUsuario);

        // Si solo tiene una → entrar directo sin mostrar selector
        if (tiendas.Count == 1)
        {
            GuardarTiendaEnSession(tiendas[0].IdTienda, tiendas[0].Nombre);
            return RedirectToAction("Index", "Dashboard", new { area = "AdminCafeteria" });
        }

        // Si no tiene ninguna → error
        if (tiendas.Count == 0)
        {
            TempData["Error"] = "No tienes ninguna tienda asignada.";
            return RedirectToAction("Login", "Account", new { area = "" });
        }

        // Si tiene varias → mostrar selector
        return View(tiendas);
    }

    // POST /Tienda/AccesoTienda/ConfirmarTienda
    [HttpPost]
    [Route("AdminCafeteria/ConfirmarTienda")]
    public async Task<IActionResult> ConfirmarTienda([FromBody] ConfirmarTiendaRequest request)
    {
        int idUsuario = ObtenerIdUsuario();

        var resultado = await _seleccionarTiendaUseCase
            .EjecutarAsync(request.IdTienda, idUsuario);

        if (!resultado.Ok)
            return Json(new { ok = false, msg = resultado.Mensaje });

        GuardarTiendaEnSession(request.IdTienda, request.NombreTienda);

        // ✅ Correcto — usa Url.Action, deja que MVC resuelva la ruta
        return Json(new
        {
            ok = true,
            redirectUrl = Url.Action("Index", "Dashboard", new { area = "AdminCafeteria" })
        });
    }

    // ── Helpers ──────────────────────────────────────────────────────────
    private int ObtenerIdUsuario() =>
        int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");

    private void GuardarTiendaEnSession(int idTienda, string nombre)
    {
        HttpContext.Session.SetInt32(SessionKeyTienda, idTienda);
        HttpContext.Session.SetString(SessionKeyTiendaNombre, nombre);
    }
}

public record ConfirmarTiendaRequest(int IdTienda, string NombreTienda);