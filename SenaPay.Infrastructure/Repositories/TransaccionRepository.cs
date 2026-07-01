using Microsoft.EntityFrameworkCore;
using SenaPay.Domain.Entities;
using SenaPay.Domain.Interfaces;
using SenaPay.Infrastructure.Data;

namespace SenaPay.Infrastructure.Repositories;

public class TransaccionRepository : ITransaccionRepository
{
    private readonly SenaPayContext _context;

    public TransaccionRepository(SenaPayContext context) => _context = context;

    public async Task<Transaccione> CrearAsync(
        int idAprendiz,
        decimal total,
        DateTime fecha,
        List<(int IdProducto, int Cantidad, decimal PrecioUnitario)> detalles)
    {
        var transaccion = new Transaccione
        {
            IdAprendiz = idAprendiz,
            Total = total,
            Fecha = fecha
        };

        _context.Transacciones.Add(transaccion);
        await _context.SaveChangesAsync();

        foreach (var (idProducto, cantidad, precio) in detalles)
        {
            _context.DetalleTransaccions.Add(new DetalleTransaccion
            {
                IdTransaccion = transaccion.IdTransaccion,
                IdProducto = idProducto,
                Cantidad = cantidad,
                PrecioUnitario = precio
            });

            var producto = await _context.Productos.FindAsync(idProducto);
            if (producto != null)
            {
                producto.Stock -= cantidad;
                if (producto.Stock <= 0)
                {
                    producto.Stock = 0;
                    producto.Estado = false;
                }
            }
        }

        await _context.SaveChangesAsync();
        return transaccion;
    }

    public async Task<int> CrearCompraAtomicaAsync(
        int idAprendiz,
        int idUsuario,
        decimal saldoActual,
        decimal total,
        DateTime fecha,
        List<(int IdProducto, int Cantidad, decimal PrecioUnitario)> detalles,
        bool esReserva,
        int idTienda)
    {
        await using var tx = await _context.Database.BeginTransactionAsync();
        try
        {
            // 1. Descontar saldo al aprendiz
            var aprendiz = await _context.Aprendices.FindAsync(idAprendiz)
                ?? throw new InvalidOperationException("Aprendiz no encontrado.");
            aprendiz.Saldo -= total;

            // 2. Crear transacción principal
            var transaccion = new Transaccione
            {
                IdAprendiz   = idAprendiz,
                Total        = total,
                Fecha        = fecha,
                EsReserva    = esReserva,
                FechaRetiro  = esReserva ? fecha : null,
                EstadoReserva = esReserva ? "Pendiente" : null
            };
            _context.Transacciones.Add(transaccion);
            await _context.SaveChangesAsync();

            // 3. Detalles + reducción de stock
            foreach (var (idProducto, cantidad, precio) in detalles)
            {
                _context.DetalleTransaccions.Add(new DetalleTransaccion
                {
                    IdTransaccion  = transaccion.IdTransaccion,
                    IdProducto     = idProducto,
                    Cantidad       = cantidad,
                    PrecioUnitario = precio
                });

                var producto = await _context.Productos.FindAsync(idProducto);
                if (producto != null)
                {
                    producto.Stock -= cantidad;
                    if (producto.Stock <= 0) { producto.Stock = 0; producto.Estado = false; }
                }
            }

            // 4. Acumular ingresos en la Tienda
            var tienda = await _context.Tienda.FindAsync(idTienda);
            if (tienda != null) tienda.TotalIngresos += total;

            // 5. Registrar movimiento de saldo del aprendiz
            _context.MovimientoSaldos.Add(new MovimientoSaldo
            {
                IdUsuario       = idUsuario,
                TipoMovimiento  = "CONSUMO",
                Monto           = total,
                SaldoResultante = aprendiz.Saldo,
                Fecha           = fecha,
                Descripcion     = esReserva
                    ? $"Reserva — {detalles.Count} producto(s)"
                    : $"Compra — {detalles.Count} producto(s)",
                IdTransaccion   = transaccion.IdTransaccion
            });

            await _context.SaveChangesAsync();
            await tx.CommitAsync();
            return transaccion.IdTransaccion;
        }
        catch
        {
            await tx.RollbackAsync();
            throw;
        }
    }

    public async Task<List<Transaccione>> ObtenerPorTiendaAsync(int idTienda)
    {
        return await _context.Transacciones
            .Include(t => t.IdAprendizNavigation)
                .ThenInclude(a => a.IdUsuarioNavigation)
            .Include(t => t.DetalleTransaccions)
                .ThenInclude(d => d.IdProductoNavigation)
            .Where(t => t.DetalleTransaccions
                .Any(d => d.IdProductoNavigation!.IdTienda == idTienda))
            .OrderByDescending(t => t.Fecha)
            .Take(200)
            .ToListAsync();
    }

    public async Task<List<Transaccione>> ObtenerReservasPorTiendaAsync(int idTienda)
    {
        return await _context.Transacciones
            .Include(t => t.IdAprendizNavigation)
                .ThenInclude(a => a.IdUsuarioNavigation)
            .Include(t => t.DetalleTransaccions)
                .ThenInclude(d => d.IdProductoNavigation)
            .Where(t => t.EsReserva &&
                t.DetalleTransaccions.Any(d => d.IdProductoNavigation!.IdTienda == idTienda))
            .OrderByDescending(t => t.Fecha)
            .Take(200)
            .ToListAsync();
    }

    public async Task CambiarEstadoReservaAsync(int idTransaccion, string nuevoEstado, int idTienda)
    {
        var trans = await _context.Transacciones
            .Include(t => t.DetalleTransaccions)
                .ThenInclude(d => d.IdProductoNavigation)
            .FirstOrDefaultAsync(t => t.IdTransaccion == idTransaccion && t.EsReserva);

        if (trans == null) return;

        var pertenece = trans.DetalleTransaccions
            .Any(d => d.IdProductoNavigation?.IdTienda == idTienda);
        if (!pertenece) return;

        trans.EstadoReserva = nuevoEstado;
        await _context.SaveChangesAsync();
    }

    public async Task<decimal> ObtenerTotalVentasHoyAsync(int idTienda)
    {
        var hoy = DateTime.Today;
        return await _context.Transacciones
            .Where(t => t.Fecha >= hoy && t.Fecha < hoy.AddDays(1) &&
                t.DetalleTransaccions.Any(d => d.IdProductoNavigation!.IdTienda == idTienda))
            .SumAsync(t => (decimal?)t.Total) ?? 0m;
    }

    public async Task<int> ObtenerConteoTransaccionesHoyAsync(int idTienda)
    {
        var hoy = DateTime.Today;
        return await _context.Transacciones
            .Where(t => t.Fecha >= hoy && t.Fecha < hoy.AddDays(1) &&
                t.DetalleTransaccions.Any(d => d.IdProductoNavigation!.IdTienda == idTienda))
            .CountAsync();
    }
}
