using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SenaPay.Application.UseCases.AdminTienda;
using System.Security.Claims;

namespace SENA_PAY__PRUEBAS.Areas.AdminCafeteria.Controllers;

[Area("AdminCafeteria")]
[Authorize(AuthenticationSchemes = "SenaPayCookies")]
public class DashboardController : Controller
{
    private readonly ObtenerDashboardTiendaUseCase _dashboardUC;
    private readonly GestionarCategoriasTiendaUseCase _categoriasUC;

    public DashboardController(
        ObtenerDashboardTiendaUseCase dashboardUC,
        GestionarCategoriasTiendaUseCase categoriasUC)
    {
        _dashboardUC = dashboardUC;
        _categoriasUC = categoriasUC;
    }

    // Helper seguro — nunca lanza NullReferenceException
    private int? GetIdTienda()
    {
        return HttpContext.Session.GetInt32(AccesoTiendaController.SessionKeyTienda);
    }

    public async Task<IActionResult> Index()
    {
        var idTienda = GetIdTienda();

        // El AdminCafeteria aún no tiene tienda asignada
        if (idTienda is null)
            return RedirectToAction("SeleccionarTienda", "AccesoTienda",
            new { area = "AdminCafeteria" });

        var nombre = User.FindFirstValue(ClaimTypes.Name) ?? "Admin";
        var dto = await _dashboardUC.EjecutarAsync(idTienda.Value, nombre);
        return View(dto);
    }

    // Vista amigable cuando el admin no tiene tienda todavía
    public IActionResult SinTienda() => View();

    public async Task<IActionResult> SeleccionarCategorias()
    {
        var idTienda = GetIdTienda();
        if (idTienda is null) return RedirectToAction("SeleccionarTienda", "AccesoTienda", new { area = "AdminCafeteria" });

        var categorias = await _categoriasUC.ObtenerConEstadoAsync(idTienda.Value);
        return View(categorias);
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> GuardarCategorias([FromForm] List<int> idsSeleccionados)
    {
        var idTienda = GetIdTienda();
        if (idTienda is null) return RedirectToAction("SinTienda");

        await _categoriasUC.GuardarSeleccionAsync(idTienda.Value, idsSeleccionados);
        TempData["Exito"] = "Categorías guardadas correctamente.";
        return RedirectToAction(nameof(Index));
    }
}