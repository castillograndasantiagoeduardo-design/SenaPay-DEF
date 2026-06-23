using Microsoft.EntityFrameworkCore;
using SenaPay.Domain.Entities;
using SenaPay.Domain.Interfaces;
using SenaPay.Infrastructure.Data;

namespace SenaPay.Infrastructure.Repositories.Tienda;

public class TiendaCategoriaRepository : ITiendaCategoriaRepository
{
    private readonly SenaPayContext _context;

    public TiendaCategoriaRepository(SenaPayContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<int>> ObtenerIdsHabilitadosPorTiendaAsync(int idTienda)
    {
        return await _context.TiendaCategoria
            .Where(tc => tc.IdTienda == idTienda && tc.Habilitada)
            .Select(tc => tc.IdCategoria)
            .ToListAsync();
    }

    public async Task HabilitarCategoriasAsync(int idTienda, IEnumerable<int> idsCategorias)
    {
        foreach (var idCategoria in idsCategorias)
        {
            var existente = await _context.TiendaCategoria
                .FirstOrDefaultAsync(tc => tc.IdTienda == idTienda && tc.IdCategoria == idCategoria);

            if (existente == null)
            {
                _context.TiendaCategoria.Add(new TiendaCategoria
                {
                    IdTienda = idTienda,
                    IdCategoria = idCategoria,
                    Habilitada = true
                });
            }
            else
            {
                existente.Habilitada = true;
            }
        }
        await _context.SaveChangesAsync();
    }

    public async Task DeshabilitarCategoriaAsync(int idTienda, int idCategoria)
    {
        var registro = await _context.TiendaCategoria
            .FirstOrDefaultAsync(tc => tc.IdTienda == idTienda && tc.IdCategoria == idCategoria);

        if (registro != null)
        {
            registro.Habilitada = false;
            await _context.SaveChangesAsync();
        }
    }

    public async Task<bool> TieneCategoriasAsync(int idTienda)
    {
        return await _context.TiendaCategoria
            .AnyAsync(tc => tc.IdTienda == idTienda && tc.Habilitada);
    }
}