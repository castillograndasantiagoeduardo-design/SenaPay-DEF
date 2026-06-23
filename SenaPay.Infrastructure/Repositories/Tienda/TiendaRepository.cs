using Microsoft.EntityFrameworkCore;
using SenaPay.Domain.Entities;
using SenaPay.Domain.Interfaces;
using SenaPay.Infrastructure.Data;

namespace SenaPay.Infrastructure.Repositories.Usuarios;

public class TiendaRepository : ITiendaRepository
{
    private readonly SenaPayContext _context;
    public TiendaRepository(SenaPayContext context) => _context = context;

    public async Task<IEnumerable<Tiendum>> ObtenerTodasAsync() =>
        await _context.Tienda
            .Include(t => t.IdSedeNavigation)
            .Include(t => t.IdAdminCafeteriaNavigation)
            .ToListAsync();

    public async Task<IEnumerable<Tiendum>> ObtenerPorSedeAsync(int idSede) =>
        await _context.Tienda
            .Include(t => t.IdSedeNavigation)
            .Include(t => t.IdAdminCafeteriaNavigation)
            .Where(t => t.IdSede == idSede)
            .ToListAsync();

    public async Task<Tiendum?> ObtenerPorIdAsync(int idTienda) =>
        await _context.Tienda
            .Include(t => t.IdSedeNavigation)
            .Include(t => t.IdAdminCafeteriaNavigation)
            .FirstOrDefaultAsync(t => t.IdTienda == idTienda);

    public async Task<Tiendum?> ObtenerPorAdminAsync(int idUsuario) =>
        await _context.Tienda
            .Include(t => t.IdAdminCafeteriaNavigation)
            .FirstOrDefaultAsync(t =>
                t.IdAdminCafeteriaNavigation != null &&
                t.IdAdminCafeteriaNavigation.IdUsuario == idUsuario);

    // Implementación en TiendaRepository
    public async Task<List<Tiendum>> ObtenerPorAdminUsuarioAsync(int idUsuario) =>
        await _context.Tienda
            .Include(t => t.IdSedeNavigation)
            .Include(t => t.IdAdminCafeteriaNavigation)
            .Where(t => t.IdAdminCafeteriaNavigation != null &&
                        t.IdAdminCafeteriaNavigation.IdUsuario == idUsuario)
            .ToListAsync();

    public async Task CrearAsync(Tiendum tienda) =>
        await _context.Tienda.AddAsync(tienda);

    public async Task GuardarCambiosAsync() =>
        await _context.SaveChangesAsync();
}