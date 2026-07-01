using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;          // ✅ IFormFile aquí sí aplica
using SenaPay.Application.DTOs.Producto;
using SenaPay.Application.UseCases.AdminTienda;
using System.Security.Claims;

namespace SENA_PAY__PRUEBAS.Areas.AdminCafeteria.Controllers;

[Area("AdminCafeteria")]
[Authorize(Roles = "3")]
public class InventarioController : Controller
{
    private readonly GestionarProductoUseCase _productoUC;
    private readonly GestionarCategoriasTiendaUseCase _categoriasUC;
    private readonly IWebHostEnvironment _env;

    public InventarioController(
    GestionarProductoUseCase productoUC,
    GestionarCategoriasTiendaUseCase categoriasUC,
    IWebHostEnvironment env)
    {
        _productoUC = productoUC;
        _categoriasUC = categoriasUC;
        _env = env;
    }

    private int? GetIdTienda()
    {
        return HttpContext.Session.GetInt32(
            AccesoTiendaController.SessionKeyTienda);
    }

    public async Task<IActionResult> Index(int? categoriaFiltro)
    {
        var idTienda = GetIdTienda();
        if (idTienda is null)
            return RedirectToAction("SeleccionarTienda", "AccesoTienda", new { area = "AdminCafeteria" });

        ViewBag.Productos = await _productoUC.ObtenerInventarioAsync(idTienda.Value, categoriaFiltro);
        ViewBag.Categorias = await _categoriasUC.ObtenerConEstadoAsync(idTienda.Value);
        ViewBag.CategoriaFiltro = categoriaFiltro;

        // Para el badge del sidebar
        ViewBag.ProductosBajoStock = await _productoUC.ContarBajoStockAsync(idTienda.Value);

        return View();
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Crear(CrearProductoRequest request, IFormFile? ImagenFile)
    {
        var idTienda = GetIdTienda();
        if (idTienda is null) return RedirectToAction("SinTienda", "Dashboard");

        request.IdTienda = idTienda.Value;

        // ── Guardar imagen si viene ────────────────────────────
        if (ImagenFile is { Length: > 0 })
            request.Imagen = await GuardarImagenAsync(ImagenFile);

        if (!ModelState.IsValid) return RedirectToAction(nameof(Index));

        var error = await _productoUC.CrearAsync(request);
        if (error != null) { TempData["Error"] = error; return RedirectToAction(nameof(Index)); }
        TempData["Exito"] = "Producto creado correctamente.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Editar(EditarProductoRequest request, IFormFile? ImagenFile)
    {
        if (ImagenFile is { Length: > 0 })
            request.Imagen = await GuardarImagenAsync(ImagenFile);

        if (!ModelState.IsValid) return RedirectToAction(nameof(Index));
        var error = await _productoUC.EditarAsync(request);
        if (error != null) { TempData["Error"] = error; return RedirectToAction(nameof(Index)); }
        TempData["Exito"] = "Producto actualizado.";
        return RedirectToAction(nameof(Index));
    }


    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Eliminar(int id)
    {
        await _productoUC.EliminarAsync(id);
        TempData["Exito"] = "Producto eliminado.";
        return RedirectToAction(nameof(Index));
    }

    // ── Helper privado ─────────────────────────────────────────
    private async Task<string> GuardarImagenAsync(IFormFile archivo)
    {
        string webRoot = _env.WebRootPath
                      ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
        string carpeta = Path.Combine(webRoot, "uploads", "productos");
        Directory.CreateDirectory(carpeta);

        string ext = Path.GetExtension(archivo.FileName).ToLowerInvariant();
        string nombreUnico = $"{Guid.NewGuid()}{ext}";
        string rutaFisica = Path.Combine(carpeta, nombreUnico);

        await using var stream = System.IO.File.Create(rutaFisica);
        await archivo.CopyToAsync(stream);

        return $"/uploads/productos/{nombreUnico}";
    }
}