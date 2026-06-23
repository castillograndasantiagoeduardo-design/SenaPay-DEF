// SenaPay.Infrastructure/Repositories/CategoriaRepository.cs
using Microsoft.EntityFrameworkCore;
using SenaPay.Domain.Entities;
using SenaPay.Domain.Interfaces;
using SenaPay.Infrastructure.Data; // tu SenaPayContext

namespace SenaPay.Infrastructure.Repositories;

public class CategoriaRepository : ICategoriaRepository
{
    private readonly SenaPayContext _context;

    public CategoriaRepository(SenaPayContext context)
        => _context = context;

    public async Task<IEnumerable<Categoria>> ObtenerTodasAsync()
        => await _context.Categoria.AsNoTracking().ToListAsync();

    public async Task<Categoria?> ObtenerPorIdAsync(int id)
        => await _context.Categoria.FindAsync(id);

    public async Task<bool> ExisteConNombreAsync(string nombre)
        => await _context.Categoria
            .AnyAsync(c => c.Nombre_Categoria.ToLower() == nombre.ToLower().Trim());

    public async Task AgregarAsync(Categoria categoria)
        => await _context.Categoria.AddAsync(categoria);

    public async Task EliminarAsync(Categoria categoria)
    {
        _context.Categoria.Remove(categoria);
        await Task.CompletedTask;
    }

    public async Task<bool> ExisteConNombreExcluyendoIdAsync(string nombre, int idExcluir)
    => await _context.Categoria
        .AnyAsync(c => c.Nombre_Categoria.ToLower() == nombre.ToLower().Trim()
                    && c.Id_Categoria != idExcluir);

    public async Task GuardarCambiosAsync()
        => await _context.SaveChangesAsync();

    public async Task<IEnumerable<Categoria>> ObtenerPorIdsAsync(IEnumerable<int> ids)
    => await _context.Categoria
        .Where(c => ids.Contains(c.Id_Categoria))
        .AsNoTracking()
        .ToListAsync();
}