using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SenaPay.Application.UseCases.AdminTienda;

namespace SENA_PAY__PRUEBAS.Areas.AdminCafeteria.Controllers;

[Area("AdminCafeteria")]
[Authorize(Roles = "3")]
public class ReservasController : Controller
{
    private readonly ObtenerReservasTiendaUseCase _reservasUC;
    private readonly CambiarEstadoReservaUseCase _estadoUC;

    public ReservasController(
        ObtenerReservasTiendaUseCase reservasUC,
        CambiarEstadoReservaUseCase estadoUC)
    {
        _reservasUC = reservasUC;
        _estadoUC   = estadoUC;
    }

    public async Task<IActionResult> Index()
    {
        var idTienda = HttpContext.Session.GetInt32(AccesoTiendaController.SessionKeyTienda);
        if (idTienda == null)
            return RedirectToAction("SeleccionarTienda", "AccesoTienda", new { area = "AdminCafeteria" });

        ViewBag.Reservas = await _reservasUC.EjecutarAsync(idTienda.Value);
        return View();
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> CambiarEstado(int idTransaccion, string estado)
    {
        var idTienda = HttpContext.Session.GetInt32(AccesoTiendaController.SessionKeyTienda);
        if (idTienda == null) return RedirectToAction(nameof(Index));

        var (ok, msg) = await _estadoUC.EjecutarAsync(idTransaccion, estado, idTienda.Value);
        TempData[ok ? "Exito" : "Error"] = msg;
        return RedirectToAction(nameof(Index));
    }
}
