using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SenaPay.Application.DTOs.Carrito;
using SenaPay.Application.UseCases.Sedes;
using SenaPay.Application.UseCases.Tienda;
using SENA_PAY__PRUEBAS.Filters;
using System.Security.Claims;
using System.Text.Json;

namespace SENA_PAY__PRUEBAS.Areas.Tiendas.Controllers;

[Area("Tiendas")]
[Authorize(Roles = "1,2")]
[NoCache]
public class TiendaController : Controller
{
    private const string SessionKeyCarrito    = "sp-carrito";
    private const string SessionKeySede       = "sp-sede-id";
    private const string SessionKeySedeNombre = "sp-sede-nombre";
    private const string SessionKeyTiendaId   = "sp-tienda-id";
    private const string SessionKeyTiendaNombre = "sp-tienda-nombre";

    private static readonly JsonSerializerOptions _jsonOpts = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    private readonly ObtenerSedesUseCase _sedesUC;
    private readonly ObtenerProductosPorSedeUseCase _productosUC;
    private readonly RealizarCompraUseCase _compraUC;
    private readonly GetPerfilFuncionarioUseCase _perfilFuncUC;
    private readonly VerificarCompradorUseCase _verificarUC;

    public TiendaController(
        ObtenerSedesUseCase sedesUC,
        ObtenerProductosPorSedeUseCase productosUC,
        RealizarCompraUseCase compraUC,
        GetPerfilFuncionarioUseCase perfilFuncUC,
        VerificarCompradorUseCase verificarUC)
    {
        _sedesUC      = sedesUC;
        _productosUC  = productosUC;
        _compraUC     = compraUC;
        _perfilFuncUC = perfilFuncUC;
        _verificarUC  = verificarUC;
    }

    // GET /Tienda  — Selección de sede
    public async Task<IActionResult> Index()
    {
        var sedes = await _sedesUC.EjecutarAsync();
        ViewBag.NombreUsuario = User.FindFirstValue(ClaimTypes.Name) ?? "Usuario";
        ViewBag.EsAprendiz = User.FindFirstValue(ClaimTypes.Role) == "1";
        return View(sedes);
    }

    // GET /Tienda/Catalogo?idSede=1&categoria=Bebidas&busqueda=cafe
    public async Task<IActionResult> Catalogo(int idSede, string? categoria, string? busqueda)
    {
        if (idSede <= 0)
            return RedirectToAction(nameof(Index));

        var carrito = LeerCarrito();
        var (vm, nombreTienda, idTienda) = await _productosUC.EjecutarAsync(
            idSede, categoria, busqueda, carrito.Count);

        if (vm == null)
        {
            TempData["Error"] = "Esta sede no tiene tienda disponible actualmente.";
            return RedirectToAction(nameof(Index));
        }

        HttpContext.Session.SetInt32(SessionKeySede, idSede);
        HttpContext.Session.SetInt32(SessionKeyTiendaId, idTienda);
        if (nombreTienda != null)
            HttpContext.Session.SetString(SessionKeyTiendaNombre, nombreTienda);

        var sedes = await _sedesUC.EjecutarAsync();
        var sede = sedes.FirstOrDefault(s => s.IdSede == idSede);
        var sedeNombre = sede != null ? $"{sede.Nombre} — {sede.Ciudad}" : $"Sede {idSede}";
        if (sede != null)
            HttpContext.Session.SetString(SessionKeySedeNombre, sedeNombre);

        ViewBag.IdSede       = idSede;
        ViewBag.SedeNombre   = sedeNombre;
        ViewBag.TiendaNombre = nombreTienda ?? "Tienda";
        ViewBag.NombreUsuario = User.FindFirstValue(ClaimTypes.Name) ?? "Usuario";
        ViewBag.EsAprendiz   = User.FindFirstValue(ClaimTypes.Role) == "1";
        ViewBag.Carrito      = carrito;

        return View(vm);
    }

    // GET /Tienda/CarnetFuncionario
    [Authorize(Roles = "2")]
    public async Task<IActionResult> CarnetFuncionario()
    {
        var docStr = User.FindFirstValue("Documento");
        if (!int.TryParse(docStr, out int documento))
            return RedirectToAction("Login", "Account", new { area = "Account" });

        var perfil = await _perfilFuncUC.EjecutarAsync(documento);
        if (perfil is null)
            return RedirectToAction("Login", "Account", new { area = "Account" });

        return View(perfil);
    }

    // POST /Tienda/AgregarAlCarrito  (AJAX JSON)
    [HttpPost]
    public IActionResult AgregarAlCarrito([FromBody] AgregarAlCarritoRequest request)
    {
        if (request.IdProducto <= 0 || request.Precio <= 0)
            return Json(new { ok = false, msg = "Datos de producto inválidos." });

        var carrito = LeerCarrito();
        var existente = carrito.FirstOrDefault(i => i.IdProducto == request.IdProducto);

        if (existente != null)
            existente.Cantidad++;
        else
            carrito.Add(new ItemCarritoDto
            {
                IdProducto     = request.IdProducto,
                IdTienda       = request.IdTienda,
                NombreProducto = request.NombreProducto,
                Precio         = request.Precio,
                Cantidad       = 1,
                Imagen         = request.Imagen,
                NombreCategoria = request.NombreCategoria
            });

        GuardarCarrito(carrito);
        return Json(new { ok = true, total = carrito.Count, items = carrito });
    }

    // POST /Tienda/ActualizarCarrito  (AJAX JSON)
    [HttpPost]
    public IActionResult ActualizarCarrito([FromBody] ActualizarCarritoRequest request)
    {
        var carrito = LeerCarrito();
        var item = carrito.FirstOrDefault(i => i.IdProducto == request.IdProducto);

        if (item == null)
            return Json(new { ok = false, msg = "Producto no encontrado en el carrito." });

        if (request.Delta == 0 || item.Cantidad + request.Delta <= 0)
            carrito.Remove(item);
        else
            item.Cantidad += request.Delta;

        GuardarCarrito(carrito);
        var subtotal = carrito.Sum(i => i.Subtotal);
        return Json(new { ok = true, total = carrito.Count, items = carrito, subtotal });
    }

    // GET /Tienda/ObtenerCarrito  (AJAX)
    [HttpGet]
    public IActionResult ObtenerCarrito()
    {
        var carrito = LeerCarrito();
        return Json(new { items = carrito, total = carrito.Count, subtotal = carrito.Sum(i => i.Subtotal) });
    }

    // GET /Tienda/VerificarComprador?doc=X  (AJAX — solo Funcionario)
    [HttpGet]
    [Authorize(Roles = "2")]
    public async Task<IActionResult> VerificarComprador(int doc)
    {
        if (doc <= 0)
            return Json(new { encontrado = false, mensaje = "Documento inválido." });

        var (encontrado, nombre, saldo) = await _verificarUC.EjecutarAsync(doc);
        return Json(new { encontrado, nombre, saldo });
    }

    // POST /Tienda/Pagar  (AJAX JSON)
    [HttpPost]
    public async Task<IActionResult> Pagar([FromBody] PagarRequest request)
    {
        var carrito = LeerCarrito();
        if (carrito.Count == 0)
            return Json(new { ok = false, msg = "El carrito está vacío." });

        var rol = User.FindFirstValue(ClaimTypes.Role);
        var ci  = new System.Globalization.CultureInfo("es-CO");

        // Funcionario con comprador escaneado → pago real a cuenta del aprendiz
        if (rol == "2" && request.DocumentoComprador.HasValue)
        {
            var resultado = await _compraUC.EjecutarAsync(
                request.DocumentoComprador.Value, carrito, request.FechaReserva);

            if (resultado.Ok)
                GuardarCarrito(new List<ItemCarritoDto>());

            return Json(resultado);
        }

        // Funcionario sin scan → solo reserva (sin descuento de saldo)
        if (rol == "2")
        {
            GuardarCarrito(new List<ItemCarritoDto>());
            var fecha = request.FechaReserva;
            var msg = fecha.HasValue
                ? $"¡Reserva registrada para el {fecha.Value.ToString("dd/MM/yyyy", ci)} a las {fecha.Value.ToString("HH:mm", ci)}. Preséntate en la tienda."
                : "¡Pedido registrado! Preséntate en la tienda para recogerlo.";

            return Json(new ResultadoPagoDto
            {
                Ok = true,
                Mensaje = msg,
                EsReserva = true,
                FechaReservaFormateada = fecha?.ToString("dd/MM/yyyy HH:mm", ci)
            });
        }

        // Aprendiz — pago real con saldo propio
        var docStr = User.FindFirstValue("Documento");
        if (!int.TryParse(docStr, out int documento))
            return Json(new { ok = false, msg = "Sesión inválida. Inicia sesión de nuevo." });

        var res = await _compraUC.EjecutarAsync(documento, carrito, request.FechaReserva);

        if (res.Ok)
            GuardarCarrito(new List<ItemCarritoDto>());

        return Json(res);
    }

    // ── Helpers de carrito (Session JSON) ────────────────────────────────────

    private List<ItemCarritoDto> LeerCarrito()
    {
        var json = HttpContext.Session.GetString(SessionKeyCarrito);
        if (string.IsNullOrEmpty(json))
            return new List<ItemCarritoDto>();
        try
        {
            return JsonSerializer.Deserialize<List<ItemCarritoDto>>(json, _jsonOpts)
                   ?? new List<ItemCarritoDto>();
        }
        catch
        {
            return new List<ItemCarritoDto>();
        }
    }

    private void GuardarCarrito(List<ItemCarritoDto> carrito)
    {
        HttpContext.Session.SetString(
            SessionKeyCarrito,
            JsonSerializer.Serialize(carrito, _jsonOpts));
    }
}
