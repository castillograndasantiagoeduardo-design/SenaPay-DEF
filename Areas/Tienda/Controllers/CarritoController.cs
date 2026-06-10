using Microsoft.AspNetCore.Mvc;
using SenaPay.Application.UseCases.Tienda;
using SenaPay.Domain.Interfaces.Tienda.DTOs;
using System.Text.Json;

namespace SENA_PAY__PRUEBAS.Areas.Tienda.Controllers;

/// <summary>
/// Responsabilidades de este controlador:
/// - Leer y escribir el carrito en Session (infraestructura web).
/// - Delegar la lógica a los casos de uso.
/// - Retornar la vista o redirección correcta.
/// 
/// NO contiene lógica de negocio.
/// NO importa repositorios ni SenaPayContext.
/// </summary>
[Area("Tienda")]
public class CarritoController : Controller
{
    // ── Clave de sesión ───────────────────────────────────────────
    private const string CarritoSessionKey = "Carrito_SenaPay";

    // ── Casos de uso inyectados ───────────────────────────────────
    private readonly AgregarAlCarritoUseCase _agregarUseCase;
    private readonly ProcesarCompraUseCase _procesarCompraUseCase;

    public CarritoController(
        AgregarAlCarritoUseCase agregarUseCase,
        ProcesarCompraUseCase procesarCompraUseCase)
    {
        _agregarUseCase = agregarUseCase;
        _procesarCompraUseCase = procesarCompraUseCase;
    }

    // ── GET /Carrito ──────────────────────────────────────────────
    public IActionResult Index()
    {
        var carrito = ObtenerCarritoDeSesion();
        return View(carrito);
    }

    // ── POST /Carrito/Agregar ─────────────────────────────────────
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Agregar(int idProducto, int cantidad = 1)
    {
        var carritoActual = ObtenerCarritoDeSesion();

        // Delega la lógica al caso de uso
        var (carritoActualizado, ok, mensaje) =
            await _agregarUseCase.EjecutarAsync(carritoActual, idProducto, cantidad);

        if (ok)
        {
            GuardarCarritoEnSesion(carritoActualizado);
            TempData["Mensaje"] = mensaje;
            TempData["MensajeTipo"] = "success";
        }
        else
        {
            TempData["Mensaje"] = mensaje;
            TempData["MensajeTipo"] = "error";
        }

        return RedirectToAction("Index", "Productos");
    }

    // ── POST /Carrito/Eliminar ────────────────────────────────────
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Eliminar(int idProducto)
    {
        var carrito = ObtenerCarritoDeSesion();
        carrito.Items.RemoveAll(i => i.Id_Producto == idProducto);
        GuardarCarritoEnSesion(carrito);

        return RedirectToAction("Index");
    }

    // ── POST /Carrito/Vaciar ──────────────────────────────────────
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Vaciar()
    {
        HttpContext.Session.Remove(CarritoSessionKey);
        return RedirectToAction("Index");
    }

    // ── POST /Carrito/Confirmar ───────────────────────────────────
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Confirmar()
    {
        // El idAprendiz viene de la sesión de autenticación
        var docStr = HttpContext.Session.GetString("UsuarioDoc");
        if (!int.TryParse(docStr, out int idAprendiz))
            return RedirectToAction("Login", "Account");

        var carrito = ObtenerCarritoDeSesion();

        // Delega toda la lógica de compra al caso de uso
        var resultado = await _procesarCompraUseCase.EjecutarAsync(carrito, idAprendiz);

        if (resultado.Exitosa)
        {
            // Limpiar el carrito solo si la compra fue exitosa
            HttpContext.Session.Remove(CarritoSessionKey);
            TempData["Mensaje"] = resultado.Mensaje;
            return RedirectToAction("Confirmacion", new { id = resultado.IdTransaccion });
        }

        // Si falla, vuelve al carrito con el mensaje de error
        TempData["Error"] = resultado.Mensaje;
        return RedirectToAction("Index");
    }

    // ── GET /Carrito/Confirmacion ─────────────────────────────────
    public IActionResult Confirmacion(int id)
    {
        // Muestra el resumen de la transacción completada
        // (puedes agregar ObtenerTransaccionUseCase si necesitas más detalle)
        ViewBag.IdTransaccion = id;
        ViewBag.Mensaje = TempData["Mensaje"];
        return View();
    }

    // ── Helpers de sesión (responsabilidad de Presentación) ──────
    /// <summary>
    /// Lee el carrito de la sesión HTTP y lo deserializa.
    /// Retorna un carrito vacío si no existe sesión.
    /// </summary>
    private CarritoDto ObtenerCarritoDeSesion()
    {
        var json = HttpContext.Session.GetString(CarritoSessionKey);
        return json is null
            ? new CarritoDto()
            : JsonSerializer.Deserialize<CarritoDto>(json) ?? new CarritoDto();
    }

    /// <summary>
    /// Serializa el carrito y lo guarda en la sesión HTTP.
    /// </summary>
    private void GuardarCarritoEnSesion(CarritoDto carrito)
    {
        HttpContext.Session.SetString(
            CarritoSessionKey,
            JsonSerializer.Serialize(carrito));
    }
}