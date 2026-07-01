using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SenaPay.Application.UseCases.AdminTienda;

namespace SENA_PAY__PRUEBAS.Areas.AdminCafeteria.Controllers;

[Area("AdminCafeteria")]
[Authorize(Roles = "3")]
public class VentasController : Controller
{
    private readonly ObtenerVentasTiendaUseCase _ventasUC;

    public VentasController(ObtenerVentasTiendaUseCase ventasUC)
    {
        _ventasUC = ventasUC;
    }

    public async Task<IActionResult> Index()
    {
        var idTienda = HttpContext.Session.GetInt32(AccesoTiendaController.SessionKeyTienda);
        if (idTienda == null)
            return RedirectToAction("Index", "AccesoTienda");

        var ventas = await _ventasUC.EjecutarAsync(idTienda.Value);
        ViewBag.Ventas = ventas;
        return View();
    }
}
