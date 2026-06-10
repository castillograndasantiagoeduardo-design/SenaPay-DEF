using Microsoft.AspNetCore.Mvc;
using SenaPay.Application.UseCases.Tienda;
using SenaPay.Domain.Interfaces.Tienda.DTOs;
using System.Text.Json;

namespace SENA_PAY__PRUEBAS.Areas.Tienda.Controllers;

/// <summary>
/// Responsabilidades:
/// - Leer el carrito de Session y mapearlo a CarritoDto.
/// - Obtener el idAprendiz de Session.
/// - Delegar la compra a ProcesarCompraUseCase.
/// - Limpiar la sesión solo si la compra fue exitosa.
/// - Retornar la vista o redirección correcta.
///
/// NO contiene lógica de negocio.
/// </summary>
[Area("Tienda")]
public class TransaccionController : Controller
{
    private const string CarritoSessionKey = "Carrito_SenaPay";

    private readonly ProcesarCompraUseCase _procesarCompraUseCase;

    public TransaccionController(ProcesarCompraUseCase procesarCompraUseCase)
    {
        _procesarCompraUseCase = procesarCompraUseCase;
    }

    // ── POST /Transaccion/Procesar ────────────────────────────────
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Procesar()
    {
        // ── Responsabilidad de Presentación 1: leer la sesión ─────
        var docStr = HttpContext.Session.GetString("UsuarioDoc");
        if (!int.TryParse(docStr, out int idAprendiz))
        {
            TempData["Error"] = "Sesión expirada. Por favor inicia sesión nuevamente.";
            return RedirectToAction("Login", "Account");
        }

        // ── Responsabilidad de Presentación 2: deserializar carrito ─
        var carritoDto = ObtenerCarritoDeSesion();
        if (carritoDto.Items.Count == 0)
        {
            TempData["Error"] = "Tu carrito está vacío.";
            return RedirectToAction("Index", "Carrito");
        }

        // ── Delegar toda la lógica al caso de uso ─────────────────
        var resultado = await _procesarCompraUseCase
            .EjecutarAsync(carritoDto, idAprendiz);

        if (resultado.Exitosa)
        {
            // Limpiar carrito solo tras compra exitosa
            HttpContext.Session.Remove(CarritoSessionKey);
            TempData["MensajeExito"] = resultado.Mensaje;
            return RedirectToAction("Confirmacion",
                new { id = resultado.IdTransaccion });
        }

        // Si falló, vuelve al carrito con el error
        TempData["Error"] = resultado.Mensaje;
        return RedirectToAction("Index", "Carrito");
    }

    // ── GET /Transaccion/Confirmacion/5 ──────────────────────────
    public IActionResult Confirmacion(int id)
    {
        ViewBag.IdTransaccion = id;
        ViewBag.Mensaje = TempData["MensajeExito"]
                                ?? "Compra procesada correctamente.";
        return View();
    }

    // ── Helper: leer carrito de sesión ────────────────────────────
    private CarritoDto ObtenerCarritoDeSesion()
    {
        var json = HttpContext.Session.GetString(CarritoSessionKey);
        return json is null
            ? new CarritoDto()
            : JsonSerializer.Deserialize<CarritoDto>(json) ?? new CarritoDto();
    }
}