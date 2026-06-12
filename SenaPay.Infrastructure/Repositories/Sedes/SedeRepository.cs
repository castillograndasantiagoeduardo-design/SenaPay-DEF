using Microsoft.EntityFrameworkCore;
using SenaPay.Domain.Entities;
using SenaPay.Domain.Interfaces.Core;
using SenaPay.Domain.Interfaces.Tienda;
using SenaPay.Infrastructure.Data;

namespace SenaPay.Infrastructure.Repositories.Sedes;

public class SedeRepository : ISedeRepository
{
    private readonly SenaPayContext _context;
    public SedeRepository(SenaPayContext context) => _context = context;

    public async Task<IEnumerable<Sede>> ObtenerTodasAsync() =>
        await _context.Sedes
            .Where(s => s.Estado == true)
            .OrderBy(s => s.Nombre)
            .ToListAsync();

    public async Task<Sede?> ObtenerPorIdAsync(int idSede) =>
        await _context.Sedes.FindAsync(idSede);
}