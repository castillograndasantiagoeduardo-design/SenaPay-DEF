using Microsoft.AspNetCore.Mvc;
using SenaPay.Application.UseCases.Funcionarios;
using SenaPay.Application.UseCases.Funcionarios.DTOs;

namespace SENA_PAY__PRUEBAS.Areas.Funcionarios.Controllers;

[Area("Funcionarios")]
public class TiendasController : Controller
{
    private readonly CrearTiendaUseCase _crearTiendaUseCase;
    private readonly ObtenerTiendasUseCase _obtenerTiendasUseCase;
    private readonly ObtenerSedesUseCase _obtenerSedesUseCase;

    public TiendasController(
        CrearTiendaUseCase crearTiendaUseCase,
        ObtenerTiendasUseCase obtenerTiendasUseCase,
        ObtenerSedesUseCase obtenerSedesUseCase)
    {
        _crearTiendaUseCase = crearTiendaUseCase;
        _obtenerTiendasUseCase = obtenerTiendasUseCase;
        _obtenerSedesUseCase = obtenerSedesUseCase;
    }

    // ── GET /Funcionarios/Tiendas ─────────────────────────────
    [HttpGet]
    [Route("Funcionarios/Tiendas")]
    public async Task<IActionResult> Index()
    {
        var tiendas = await _obtenerTiendasUseCase.EjecutarAsync();
        var sedes = await _obtenerSedesUseCase.EjecutarAsync();

        ViewBag.Sedes = sedes;
        return View(tiendas);
    }

    // ── POST /Funcionarios/Tiendas/Crear ─────────────────────
    [HttpPost]
    [Route("Funcionarios/Tiendas/Crear")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Crear([FromBody] CrearTiendaRequest request)
    {
        if (request is null)
            return Json(new { ok = false, msg = "Datos inválidos." });

        var resultado = await _crearTiendaUseCase.EjecutarAsync(request);
        return Json(new { ok = resultado.Ok, msg = resultado.Mensaje });
    }

    // ── GET /Funcionarios/Tiendas/ObtenerTiendas ─────────────
    [HttpGet]
    [Route("Funcionarios/Tiendas/ObtenerTiendas")]
    public async Task<IActionResult> ObtenerTiendas()
    {
        var tiendas = await _obtenerTiendasUseCase.EjecutarAsync();
        return Json(tiendas);
    }

    // ── GET /Funcionarios/Tiendas/ObtenerSedes ───────────────
    [HttpGet]
    [Route("Funcionarios/Tiendas/ObtenerSedes")]
    public async Task<IActionResult> ObtenerSedes()
    {
        var sedes = await _obtenerSedesUseCase.EjecutarAsync();
        return Json(sedes);
    }
}