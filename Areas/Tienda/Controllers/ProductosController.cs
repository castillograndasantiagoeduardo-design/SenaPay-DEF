using Microsoft.AspNetCore.Mvc;
using SenaPay.Application.UseCases.Tienda;
using SenaPay.Application.UseCases.Tienda.DTOs;
using SenaPay.Domain.Interfaces.Tienda.DTOs;
using System.Text.Json;

namespace SENA_PAY__PRUEBAS.Areas.Tienda.Controllers;

/// <summary>
/// Responsabilidades de este controlador:
/// - Leer filtros de la URL (categoría, búsqueda).
/// - Leer el carrito de Session para pasar el conteo al ViewModel.
/// - Invocar el caso de uso y pasarle el ViewModel a la vista.
///
/// NO contiene LINQ, NO filtra datos, NO navega relaciones.
/// </summary
[Area("Tienda")]
public class ProductosController : Controller
{
    private const string CarritoSessionKey = "Carrito_SenaPay";

    private readonly ObtenerProductosUseCase _obtenerProductosUseCase;
    private readonly ObtenerDetalleProductoUseCase _obtenerDetalleUseCase;

    public ProductosController(
        ObtenerProductosUseCase obtenerProductosUseCase,
        ObtenerDetalleProductoUseCase obtenerDetalleUseCase)
    {
        _obtenerProductosUseCase = obtenerProductosUseCase;
        _obtenerDetalleUseCase = obtenerDetalleUseCase;
    }

    // ── GET /Productos ────────────────────────────────────────────
    public async Task<IActionResult> Index(
        string? categoria = null,
        string? busqueda = null)
    {
        // Responsabilidad de Presentación: leer el carrito de Session
        int itemsEnCarrito = ContarItemsCarrito();

        // Delegar toda la lógica al caso de uso
        var viewModel = await _obtenerProductosUseCase
            .EjecutarAsync(categoria, busqueda, itemsEnCarrito);

        return View(viewModel);
    }

    // ── GET /Productos/Detalle/5 ──────────────────────────────────
    public async Task<IActionResult> Detalle(int id)
    {
        var producto = await _obtenerDetalleUseCase.EjecutarAsync(id);

        if (producto is null)
            return NotFound();

        return View(producto);
    }

    // ── Helper: contar ítems del carrito desde Session ────────────
    private int ContarItemsCarrito()
    {
        try
        {
            var json = HttpContext.Session.GetString(CarritoSessionKey);
            if (string.IsNullOrEmpty(json)) return 0;

            var carrito = JsonSerializer.Deserialize<CarritoDto>(json);
            return carrito?.TotalItems ?? 0;
        }
        catch
        {
            return 0;
        }
    }
}