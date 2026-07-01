using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SenaPay.Application.DTOs.Carrito;
using SenaPay.Application.UseCases.Tienda;
using SenaPay.Domain.Interfaces;

namespace SENA_PAY__PRUEBAS.Areas.AdminCafeteria.Controllers;

[Area("AdminCafeteria")]
[Authorize(Roles = "3")]
public class CobroController : Controller
{
    private readonly IProductoRepository _productoRepo;
    private readonly VerificarCompradorUseCase _verificarUC;
    private readonly RealizarCompraUseCase _compraUC;

    public CobroController(
        IProductoRepository productoRepo,
        VerificarCompradorUseCase verificarUC,
        RealizarCompraUseCase compraUC)
    {
        _productoRepo = productoRepo;
        _verificarUC  = verificarUC;
        _compraUC     = compraUC;
    }

    public IActionResult Index() => View();

    // GET /AdminCafeteria/Cobro/BuscarProducto?q=cafe
    [HttpGet]
    public async Task<IActionResult> BuscarProducto(string q)
    {
        var idTienda = HttpContext.Session.GetInt32(AccesoTiendaController.SessionKeyTienda);
        if (idTienda == null || string.IsNullOrWhiteSpace(q))
            return Json(Array.Empty<object>());

        var todos = (await _productoRepo.ObtenerPorTiendaAsync(idTienda.Value))
            .Where(p => p.Estado && p.Stock > 0).ToList();

        var q2 = q.Trim();
        IEnumerable<SenaPay.Domain.Entities.Producto> filtrados;
        bool esEscaneo = false;

        // 1. Exact barcode match → auto-add
        var porCodigo = todos.FirstOrDefault(p =>
            !string.IsNullOrEmpty(p.CodigoBarras) &&
            p.CodigoBarras.Equals(q2, StringComparison.OrdinalIgnoreCase));
        if (porCodigo != null)
        {
            filtrados = new[] { porCodigo };
            esEscaneo = true;
        }
        // 2. Numeric → search by IdProducto
        else if (int.TryParse(q2, out int idBusq))
        {
            filtrados = todos.Where(p => p.IdProducto == idBusq);
        }
        // 3. Partial name search
        else
        {
            filtrados = todos.Where(p =>
                p.NombreProducto.Contains(q2, StringComparison.OrdinalIgnoreCase));
        }

        return Json(filtrados.Select(p => new
        {
            p.IdProducto,
            p.NombreProducto,
            p.Precio,
            p.Stock,
            p.Imagen,
            esEscaneo
        }));
    }

    // GET /AdminCafeteria/Cobro/VerificarComprador?doc=X
    [HttpGet]
    public async Task<IActionResult> VerificarComprador(int doc)
    {
        if (doc <= 0) return Json(new { encontrado = false });
        var (encontrado, nombre, saldo) = await _verificarUC.EjecutarAsync(doc);
        return Json(new { encontrado, nombre, saldo });
    }

    // POST /AdminCafeteria/Cobro/Cobrar
    [HttpPost]
    public async Task<IActionResult> Cobrar([FromBody] CobroRequest request)
    {
        if (request.Items.Count == 0)
            return Json(new { ok = false, mensaje = "No hay productos en el pedido." });
        if (request.DocumentoComprador <= 0)
            return Json(new { ok = false, mensaje = "Debes escanear el carnet del aprendiz." });

        var carrito = new List<ItemCarritoDto>();
        foreach (var item in request.Items)
        {
            var producto = await _productoRepo.ObtenerPorIdAsync(item.IdProducto);
            if (producto == null || !producto.Estado)
                return Json(new { ok = false, mensaje = $"'{item.NombreProducto}' ya no está disponible." });
            if (producto.Stock < item.Cantidad)
                return Json(new { ok = false, mensaje = $"Stock insuficiente para '{producto.NombreProducto}'. Quedan {producto.Stock}." });

            carrito.Add(new ItemCarritoDto
            {
                IdProducto      = producto.IdProducto,
                IdTienda        = producto.IdTienda ?? 0,
                NombreProducto  = producto.NombreProducto,
                Precio          = producto.Precio,
                Cantidad        = item.Cantidad
            });
        }

        var resultado = await _compraUC.EjecutarAsync(request.DocumentoComprador, carrito, null);
        return Json(new
        {
            ok               = resultado.Ok,
            mensaje          = resultado.Mensaje,
            saldoRestante    = resultado.SaldoRestante,
            idTransaccion    = resultado.IdTransaccion
        });
    }
}

public class CobroRequest
{
    public List<CobroItem> Items { get; set; } = new();
    public int DocumentoComprador { get; set; }
}

public class CobroItem
{
    public int    IdProducto    { get; set; }
    public int    Cantidad      { get; set; }
    public string NombreProducto { get; set; } = string.Empty;
}
