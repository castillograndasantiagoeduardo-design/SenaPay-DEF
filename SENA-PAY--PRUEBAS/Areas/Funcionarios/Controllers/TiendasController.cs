// Areas/Funcionarios/Controllers/TiendasController.cs
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SenaPay.Application.DTOs.Tienda;
using SenaPay.Application.UseCases.Sedes;
using SenaPay.Application.UseCases.Tienda;

namespace SENA_PAY__PRUEBAS.Areas.Funcionarios.Controllers;

[Area("Funcionarios")]
[Authorize(Roles = "2")]
public class TiendasController : Controller
{
    private readonly CrearTiendaUseCase _crearTiendaUseCase;
    private readonly ObtenerTiendasUseCase _obtenerTiendasUseCase;
    private readonly ObtenerSedesUseCase _obtenerSedesUseCase;
    private readonly ObtenerAdminsDisponiblesUseCase _obtenerAdminsUseCase;
    private readonly AsignarAdminTiendaUseCase _asignarAdminUseCase;

    public TiendasController(
        CrearTiendaUseCase crearTiendaUseCase,
        ObtenerTiendasUseCase obtenerTiendasUseCase,
        ObtenerSedesUseCase obtenerSedesUseCase,
        ObtenerAdminsDisponiblesUseCase obtenerAdminsUseCase,
        AsignarAdminTiendaUseCase asignarAdminUseCase)
    {
        _crearTiendaUseCase = crearTiendaUseCase;
        _obtenerTiendasUseCase = obtenerTiendasUseCase;
        _obtenerSedesUseCase = obtenerSedesUseCase;
        _obtenerAdminsUseCase = obtenerAdminsUseCase;
        _asignarAdminUseCase = asignarAdminUseCase;
    }

    // GET /Funcionarios/Tiendas ─────────────────────────────────────────────
    [HttpGet]
    [Route("Funcionarios/Tiendas")]
    public async Task<IActionResult> Index()
    {
        var tiendas = await _obtenerTiendasUseCase.EjecutarAsync();
        var sedes = await _obtenerSedesUseCase.EjecutarAsync();
        var admins = await _obtenerAdminsUseCase.EjecutarAsync();

        ViewBag.Sedes = sedes;
        ViewBag.Admins = admins; // lista de AdminCafeteriaListItem para el modal
        return View(tiendas);
    }

    // POST /Funcionarios/Tiendas/Crear ─────────────────────────────────────
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

    // GET /Funcionarios/Tiendas/ObtenerTiendas ─────────────────────────────
    [HttpGet]
    [Route("Funcionarios/Tiendas/ObtenerTiendas")]
    public async Task<IActionResult> ObtenerTiendas()
    {
        var tiendas = await _obtenerTiendasUseCase.EjecutarAsync();
        return Json(tiendas);
    }

    // GET /Funcionarios/Tiendas/ObtenerSedes ───────────────────────────────
    [HttpGet]
    [Route("Funcionarios/Tiendas/ObtenerSedes")]
    public async Task<IActionResult> ObtenerSedes()
    {
        var sedes = await _obtenerSedesUseCase.EjecutarAsync();
        return Json(sedes);
    }

    // GET /Funcionarios/Tiendas/ObtenerAdmins ──────────────────────────────
    [HttpGet]
    [Route("Funcionarios/Tiendas/ObtenerAdmins")]
    public async Task<IActionResult> ObtenerAdmins()
    {
        var admins = await _obtenerAdminsUseCase.EjecutarAsync();
        return Json(admins);
    }

    // ── POST /Funcionarios/Tiendas/AsignarAdmin ──────────────────────────
    [HttpPost]
    [Route("Funcionarios/Tiendas/AsignarAdmin")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AsignarAdmin([FromBody] AsignarAdminRequest request)
    {
        if (request is null)
            return Json(new { ok = false, msg = "Datos inválidos." });

        var resultado = await _asignarAdminUseCase.EjecutarAsync(request);
        return Json(new { ok = resultado.Ok, msg = resultado.Mensaje });
    }
}