using SenaPay.Domain.Interfaces.Tienda.DTOs;
using SenaPay.Domain.Entities;
using SenaPay.Domain.Interfaces.Tienda;

namespace SenaPay.Application.UseCases.Tienda;

/// <summary>
/// Caso de uso: Procesar la compra completa del carrito.
///
/// Responsabilidades (en orden):
/// 1. Validar que el carrito no esté vacío.
/// 2. Verificar stock de cada producto en tiempo real (no confiar solo en el carrito).
/// 3. Calcular totales y aplicar descuentos si aplica.
/// 4. Reducir el stock de cada producto vendido.
/// 5. Registrar la transacción en la base de datos.
/// 6. Retornar ResultadoCompra con éxito o mensaje de error.
/// </summary>
public class ProcesarCompraUseCase
{
    private readonly IProductoRepository _productoRepo;
    private readonly ITransaccionRepository _transaccionRepo;

    public ProcesarCompraUseCase(
        IProductoRepository productoRepo,
        ITransaccionRepository transaccionRepo)
    {
        _productoRepo = productoRepo;
        _transaccionRepo = transaccionRepo;
    }

    /// <summary>
    /// Ejecuta el proceso completo de compra.
    /// </summary>
    /// <param name="carrito">Carrito con los ítems seleccionados.</param>
    /// <param name="idAprendiz">Aprendiz que realiza la compra.</param>
    public async Task<ResultadoCompra> EjecutarAsync(CarritoDto carrito, int idAprendiz)
    {
        // ── Regla 1: carrito no puede estar vacío ────────────────────
        if (carrito.Items.Count == 0)
            return ResultadoCompra.Fallo("El carrito está vacío.");

        // ── Regla 2: verificar stock en tiempo real ──────────────────
        // (El carrito puede tener datos desactualizados de la sesión)
        foreach (var item in carrito.Items)
        {
            bool hayStock = await _productoRepo
                .TieneStockSuficienteAsync(item.Id_Producto, item.Cantidad);

            if (!hayStock)
                return ResultadoCompra.Fallo(
                    $"Sin stock suficiente para '{item.NombreProducto}'. " +
                    "Por favor revisa tu carrito.");
        }

        // ── Regla 3: calcular total y aplicar descuentos ─────────────
        decimal totalBruto = carrito.Total;
        decimal descuento = CalcularDescuento(totalBruto, carrito.TotalItems);
        decimal totalFinal = totalBruto - descuento;

        // ── Regla 4: reducir stock de cada producto ──────────────────
        foreach (var item in carrito.Items)
        {
            bool reducido = await _productoRepo
                .ReducirStockAsync(item.Id_Producto, item.Cantidad);

            if (!reducido)
                return ResultadoCompra.Fallo(
                    $"No se pudo reservar el stock de '{item.NombreProducto}'.");
        }

        // ── Regla 5: construir y registrar la transacción ────────────
        var transaccion = new Transaccione
        {
            IdAprendiz = idAprendiz,
            Fecha = DateTime.Now,
            Total = totalFinal,
            DetalleTransaccions = carrito.Items.Select(item => new DetalleTransaccion
            {
                IdProducto = item.Id_Producto,
                Cantidad = item.Cantidad,
                PrecioUnitario = item.PrecioUnitario,
                //Subtotal = item.Subtotal
            }).ToList()
        };

        var transaccionCreada = await _transaccionRepo.CrearTransaccionAsync(transaccion);

        return ResultadoCompra.Ok(
            transaccionCreada.IdTransaccion,
            $"Compra realizada. Total: ${totalFinal:N0} COP" +
            (descuento > 0 ? $" (descuento aplicado: ${descuento:N0})" : ""));
    }

    // ── Lógica de descuentos (centralizada en la capa de Aplicación) ─
    /// <summary>
    /// Aplica descuentos por volumen o monto.
    /// Regla actual: 5% de descuento si el total supera $50.000 COP.
    /// </summary>
    private static decimal CalcularDescuento(decimal total, int totalItems)
    {
        if (total >= 50_000m) return total * 0.05m;   // 5% por monto
        if (totalItems >= 5) return total * 0.03m;   // 3% por volumen
        return 0m;
    }
}