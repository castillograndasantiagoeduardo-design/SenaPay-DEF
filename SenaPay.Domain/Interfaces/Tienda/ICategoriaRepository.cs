// ─────────────────────────────────────────────────────────────────────
// ARCHIVO NUEVO: Data/Repositories/Interfaces/ICategoriaRepository.cs
// ─────────────────────────────────────────────────────────────────────
using SenaPay.Domain.Entities;

namespace SenaPay.Domain.Interfaces.Tienda;

    public interface ICategoriaRepository
    {
        Task<IEnumerable<Categoria>> ObtenerTodasAsync();
        Task<Categoria?> ObtenerPorIdAsync(int id);
    }

