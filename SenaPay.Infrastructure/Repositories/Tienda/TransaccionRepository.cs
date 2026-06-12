using Microsoft.EntityFrameworkCore;
using SenaPay.Domain.Entities;
using SenaPay.Domain.Interfaces.Tienda;
using SenaPay.Infrastructure.Data;

namespace SenaPay.Infrastructure.Repositories.Tienda;

    public class TransaccionRepository : ITransaccionRepository
    {
        private readonly SenaPayContext _context;

        public TransaccionRepository(SenaPayContext context)
        {
            _context = context;
        }

        public async Task<Transaccione> CrearTransaccionAsync(Transaccione transaccion)
        {
            _context.Transacciones.Add(transaccion);
            await _context.SaveChangesAsync();
            return transaccion;  // EF Core rellena el Id_Transaccion generado por SQL Server
        }

        public async Task<IEnumerable<Transaccione>> ObtenerHistorialAprendizAsync(int idAprendiz)
        {
            return await _context.Transacciones
                .Include(t => t.DetalleTransaccions)
                    .ThenInclude(d => d.IdProductoNavigation)  // JOIN anidado: Transaccion → Detalles → Producto
                .Where(t => t.IdAprendiz == idAprendiz)
                .OrderByDescending(t => t.Fecha)
                .ToListAsync();
        }

        public async Task<Transaccione?> ObtenerConDetallesAsync(int idTransaccion)
        {
            return await _context.Transacciones
                .Include(t => t.DetalleTransaccions)
                    .ThenInclude(d => d.IdProductoNavigation)
                .Include(t => t.IdAprendizNavigation)
                .FirstOrDefaultAsync(t => t.IdTransaccion == idTransaccion);
        }
    }


