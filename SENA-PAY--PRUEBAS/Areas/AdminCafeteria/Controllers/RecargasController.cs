using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SenaPay.Application.UseCases.AdminTienda;
using SenaPay.Application.UseCases.Tienda;
using System.Security.Claims;

namespace SENA_PAY__PRUEBAS.Areas.AdminCafeteria.Controllers;

[Area("AdminCafeteria")]
[Authorize(Roles = "3")]
public class RecargasController : Controller
{
    private readonly RecargaSaldoUseCase _recargaUC;
    private readonly VerificarCompradorUseCase _verificarUC;

    public RecargasController(RecargaSaldoUseCase recargaUC, VerificarCompradorUseCase verificarUC)
    {
        _recargaUC   = recargaUC;
        _verificarUC = verificarUC;
    }

    public IActionResult Index() => View();

    // GET /AdminCafeteria/Recargas/BuscarAprendiz?doc=X  (AJAX)
    [HttpGet]
    public async Task<IActionResult> BuscarAprendiz(int doc)
    {
        if (doc <= 0)
            return Json(new { encontrado = false });

        var (encontrado, nombre, saldo) = await _verificarUC.EjecutarAsync(doc);
        return Json(new { encontrado, nombre, saldo });
    }

    // POST /AdminCafeteria/Recargas/Ejecutar  (AJAX JSON)
    [HttpPost]
    public async Task<IActionResult> Ejecutar([FromBody] RecargarRequest request)
    {
        if (request.Documento <= 0 || request.Monto <= 0)
            return Json(new { ok = false, mensaje = "Datos inválidos." });

        var nombreAdmin = User.FindFirstValue(ClaimTypes.Name) ?? "Admin";
        var idUsuarioStr = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0";
        int.TryParse(idUsuarioStr, out int idUsuario);

        var (ok, mensaje, nuevoSaldo, nombreAprendiz) = await _recargaUC.EjecutarAsync(
            request.Documento, request.Monto, nombreAdmin, idUsuario);

        return Json(new { ok, mensaje, nuevoSaldo, nombreAprendiz });
    }
}

public class RecargarRequest
{
    public int Documento { get; set; }
    public decimal Monto { get; set; }
}
