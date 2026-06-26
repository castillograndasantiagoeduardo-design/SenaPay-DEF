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
}
