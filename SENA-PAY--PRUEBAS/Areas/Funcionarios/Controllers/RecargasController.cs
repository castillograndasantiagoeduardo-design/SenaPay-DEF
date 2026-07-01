using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SenaPay.Application.UseCases.AdminTienda;
using SenaPay.Application.UseCases.Tienda;
using SENA_PAY__PRUEBAS.Filters;
using System.Security.Claims;

namespace SENA_PAY__PRUEBAS.Areas.Funcionarios.Controllers;

[Area("Funcionarios")]
[Authorize(Roles = "2")]
[NoCache]
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

    // GET /Funcionarios/Recargas/BuscarAprendiz?doc=X
    [HttpGet]
    public async Task<IActionResult> BuscarAprendiz(int doc)
    {
        if (doc <= 0) return Json(new { encontrado = false });
        var (encontrado, nombre, saldo) = await _verificarUC.EjecutarAsync(doc);
        return Json(new { encontrado, nombre, saldo });
    }

    // POST /Funcionarios/Recargas/Ejecutar
    [HttpPost]
    public async Task<IActionResult> Ejecutar([FromBody] RecargarRequestF request)
    {
        if (request.Documento <= 0 || request.Monto <= 0)
            return Json(new { ok = false, mensaje = "Datos inválidos." });

        var nombreFuncionario = User.FindFirstValue(ClaimTypes.Name) ?? "Funcionario";
        var idUsuarioStr = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0";
        int.TryParse(idUsuarioStr, out int idUsuario);

        var (ok, mensaje, nuevoSaldo, nombreAprendiz) = await _recargaUC.EjecutarAsync(
            request.Documento, request.Monto, nombreFuncionario, idUsuario);

        return Json(new { ok, mensaje, nuevoSaldo, nombreAprendiz });
    }
}

public class RecargarRequestF
{
    public int     Documento { get; set; }
    public decimal Monto     { get; set; }
}
