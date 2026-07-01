using SenaPay.Application.DTOs.Carrito;
using SenaPay.Domain.Interfaces;
using System.Globalization;

namespace SenaPay.Application.UseCases.Tienda;

public class RealizarCompraUseCase
{
    private readonly IAprendizRepository _aprendizRepo;
    private readonly IProductoRepository _productoRepo;
    private readonly ITransaccionRepository _transaccionRepo;

    public RealizarCompraUseCase(
        IAprendizRepository aprendizRepo,
        IProductoRepository productoRepo,
        ITransaccionRepository transaccionRepo)
    {
        _aprendizRepo = aprendizRepo;
        _productoRepo = productoRepo;
        _transaccionRepo = transaccionRepo;
    }

    public async Task<ResultadoPagoDto> EjecutarAsync(
        int documento,
        List<ItemCarritoDto> carrito,
        DateTime? fechaReserva)
    {
        if (carrito.Count == 0)
            return Fallo("El carrito está vacío.");

        var aprendiz = await _aprendizRepo.ObtenerPorDocumentoAsync(documento);
        if (aprendiz == null)
            return Fallo("No se encontró tu perfil de aprendiz.");

        var total = carrito.Sum(i => i.Subtotal);

        if (aprendiz.Saldo < total)
            return Fallo(
                $"Saldo insuficiente. Tienes ${aprendiz.Saldo.ToString("N0", CultureInfo.GetCultureInfo("es-CO"))} " +
                $"y el total es ${total.ToString("N0", CultureInfo.GetCultureInfo("es-CO"))}.");

        // Verificar stock de cada producto
        foreach (var item in carrito)
        {
            var producto = await _productoRepo.ObtenerPorIdAsync(item.IdProducto);
            if (producto == null || !producto.Estado)
                return Fallo($"El producto '{item.NombreProducto}' ya no está disponible.");
            if (producto.Stock < item.Cantidad)
                return Fallo($"Stock insuficiente para '{item.NombreProducto}'. Solo hay {producto.Stock} unidades.");
        }

        var fecha = fechaReserva ?? DateTime.Now;
        var detalles = carrito.Select(i => (i.IdProducto, i.Cantidad, i.Precio)).ToList();
        var ci = new CultureInfo("es-CO");

        // Calcular antes del atomic call: EF Core modifica el objeto aprendiz durante la transacción
        var saldoRestante = aprendiz.Saldo - total;

        var idTransaccion = await _transaccionRepo.CrearCompraAtomicaAsync(
            aprendiz.IdAprendiz,
            aprendiz.IdUsuarioNavigation.IdUsuario,
            aprendiz.Saldo,
            total,
            fecha,
            detalles);
        var esReserva = fechaReserva.HasValue;

        return new ResultadoPagoDto
        {
            Ok = true,
            Mensaje = esReserva
                ? $"¡Reserva confirmada! Recoge tu pedido el {fecha.ToString("dd/MM/yyyy", ci)} a las {fecha.ToString("HH:mm", ci)}."
                : "¡Compra realizada con éxito!",
            SaldoRestante = saldoRestante,
            IdTransaccion = idTransaccion,
            EsReserva = esReserva,
            FechaReservaFormateada = esReserva ? fecha.ToString("dd/MM/yyyy HH:mm", ci) : null
        };
    }

    private static ResultadoPagoDto Fallo(string mensaje) =>
        new() { Ok = false, Mensaje = mensaje };
}
