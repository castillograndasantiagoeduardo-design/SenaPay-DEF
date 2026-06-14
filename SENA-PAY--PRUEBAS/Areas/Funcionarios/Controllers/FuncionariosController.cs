using Microsoft.AspNetCore.Mvc;
using SenaPay.Application.UseCases.Funcionarios;
using SenaPay.Application.UseCases.Funcionarios.DTOs;
using SENA_PAY__PRUEBAS.Filters;

namespace SENA_PAY__PRUEBAS.Areas.Funcionarios.Controllers;

[Area("Funcionarios")]

public class FuncionariosController : Controller
{
    private readonly AgregarUsuarioUseCase _agregarUseCase;
    private readonly ObtenerUsuariosUseCase _obtenerTodosUseCase;
    private readonly ObtenerUsuarioUseCase _obtenerUnoUseCase;
    private readonly EditarUsuarioUseCase _editarUseCase;
    private readonly EliminarUsuarioUseCase _eliminarUseCase;
    private readonly ObtenerReportesUseCase _obtenerReportesUseCase;
    private readonly CambiarEstadoReporteUseCase _cambiarEstadoUseCase;
    private readonly ObtenerEstadisticasReportesUseCase _estadisticasUseCase;


    public FuncionariosController(
        AgregarUsuarioUseCase agregarUseCase,
        ObtenerUsuariosUseCase obtenerTodosUseCase,
        ObtenerUsuarioUseCase obtenerUnoUseCase,
        EditarUsuarioUseCase editarUseCase,
        EliminarUsuarioUseCase eliminarUseCase,
        ObtenerReportesUseCase obtenerReportesUseCase,
        CambiarEstadoReporteUseCase cambiarEstadoReporteUseCase,
        ObtenerEstadisticasReportesUseCase obtenerEstadisticasReportesUseCase)
    {
        _agregarUseCase = agregarUseCase;
        _obtenerTodosUseCase = obtenerTodosUseCase;
        _obtenerUnoUseCase = obtenerUnoUseCase;
        _editarUseCase = editarUseCase;
        _eliminarUseCase = eliminarUseCase;
        _obtenerReportesUseCase = obtenerReportesUseCase;
        _cambiarEstadoUseCase = cambiarEstadoReporteUseCase;
        _estadisticasUseCase = obtenerEstadisticasReportesUseCase;
    }

    public IActionResult AgregarUsuarios() => View();

    // ── AGREGAR vía JSON ──────────────────────────────────────────
    [HttpPost]
    [Route("Funcionarios/AgregarUsuarioJson")]
    [ValidateAntiforgeryTokenFromHeader]
    public async Task<IActionResult> AgregarUsuarioJson([FromBody] AgregarUsuarioRequest request)
    {
        if (request is null)
            return Json(new { ok = false, msg = "Datos inválidos." });

        var resultado = await _agregarUseCase.EjecutarAsync(request);
        return Json(new { ok = resultado.Ok, msg = resultado.Mensaje });
    }

    // ── EDITAR vía JSON ───────────────────────────────────────────
    [HttpPost]
    [Route("Funcionarios/EditarUsuarioJson")]
    [ValidateAntiforgeryTokenFromHeader]
    public async Task<IActionResult> EditarUsuarioJson([FromBody] EditarUsuarioRequest request)
    {
        if (request is null)
            return Json(new { ok = false, msg = "Datos inválidos." });

        var resultado = await _editarUseCase.EjecutarAsync(request);
        return Json(new { ok = resultado.Ok, msg = resultado.Mensaje });
    }

    // ── LISTAR ────────────────────────────────────────────────
    [HttpGet]
    [Route("Funcionarios/ObtenerUsuarios")]
    public async Task<IActionResult> ObtenerUsuarios()
    {
        var lista = await _obtenerTodosUseCase.EjecutarAsync();
        return Json(lista);
    }

    // ── OBTENER UNO ───────────────────────────────────────────
    [HttpGet]
    [Route("Funcionarios/ObtenerUsuario")]
    public async Task<IActionResult> ObtenerUsuario(int id)
    {
        var usuario = await _obtenerUnoUseCase.EjecutarAsync(id);
        return usuario is not null ? Json(usuario) : NotFound();
    }

    // ── ELIMINAR ──────────────────────────────────────────────
    [HttpPost]
    [Route("Funcionarios/EliminarUsuario")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EliminarUsuario(int idUsuario)
    {
        var resultado = await _eliminarUseCase.EjecutarAsync(idUsuario);
        return Json(new { ok = resultado.Ok, msg = resultado.Mensaje });
    }

    // ── GET: /Funcionarios/Funcionarios/Reportes ──────────────
    [HttpGet]
    public async Task<IActionResult> Reportes()
    {
        var reportes = await _obtenerReportesUseCase.EjecutarAsync();
        var estadisticas = await _estadisticasUseCase.EjecutarAsync();
        ViewBag.Estadisticas = estadisticas;
        return View(reportes);
    }

    // ── POST: Cambiar estado de un reporte (llamado por AJAX) ──
    [HttpPost]
    public async Task<IActionResult> CambiarEstadoReporte(
        int idReporte, string nuevoEstado)
    {
        try
        {
            bool ok = await _cambiarEstadoUseCase.EjecutarAsync(idReporte, nuevoEstado);
            return Json(new { ok, msg = ok ? "Estado actualizado." : "Reporte no encontrado." });
        }
        catch
        {
            return Json(new { ok = false, msg = "Error interno." });
        }
    }
}