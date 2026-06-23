using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SenaPay.Application.UseCases.AdminTienda;
using System.Security.Claims;

namespace SENA_PAY__PRUEBAS.Areas.AdminCafeteria.Controllers;

[Area("AdminCafeteria")]
[Authorize(AuthenticationSchemes = "SenaPayCookies")]
public class CategoriasController : Controller
{
    private readonly GestionarCategoriasTiendaUseCase _categoriasUC;

    public CategoriasController(GestionarCategoriasTiendaUseCase categoriasUC)
        => _categoriasUC = categoriasUC;

    private int? GetIdTienda()
    {
        return HttpContext.Session.GetInt32(
            AccesoTiendaController.SessionKeyTienda);
    }

    public async Task<IActionResult> Index()
    {
        var idTienda = GetIdTienda();
        if (idTienda is null)
            return RedirectToAction("SeleccionarTienda", "AccesoTienda", new { area = "AdminCafeteria" }); ;

        var categorias = await _categoriasUC.ObtenerConEstadoAsync(idTienda.Value);
        return View(categorias);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Toggle(int idCategoria, bool habilitar)
    {
        var idTienda = GetIdTienda();
        if (idTienda is null) return RedirectToAction("SinTienda", "Dashboard");

        await _categoriasUC.ToggleAsync(idTienda.Value, idCategoria, habilitar);

        TempData["Exito"] = habilitar
            ? "Categoría habilitada correctamente."
            : "Categoría deshabilitada correctamente.";

        return RedirectToAction(nameof(Index));
    }
}