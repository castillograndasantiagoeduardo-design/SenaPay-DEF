// ─────────────────────────────────────────────────────────────────────
// ARCHIVO NUEVO: Data/Repositories/CategoriaRepository.cs
// ─────────────────────────────────────────────────────────────────────
using Microsoft.EntityFrameworkCore;
using SenaPay.Domain.Entities;
using SenaPay.Domain.Interfaces.Tienda;
using SenaPay.Infrastructure.Data;

namespace SenaPay.Infrastructure.Repositories.Tienda;

    public class CategoriaRepository : ICategoriaRepository
    {
        private readonly SenaPayContext _context;

        public CategoriaRepository(SenaPayContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Categoria>> ObtenerTodasAsync()
            => await _context.Categoria.OrderBy(c => c.Nombre_Categoria).ToListAsync();

        public async Task<Categoria?> ObtenerPorIdAsync(int id)
            => await _context.Categoria.FindAsync(id);
    }

