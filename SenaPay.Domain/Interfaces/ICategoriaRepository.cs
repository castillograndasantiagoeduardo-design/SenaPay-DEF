// ─────────────────────────────────────────────────────────────────────
// ARCHIVO NUEVO: Data/Repositories/Interfaces/ICategoriaRepository.cs
// ─────────────────────────────────────────────────────────────────────

// ─────────────────────────────────────────────────────────────────────
// ARCHIVO NUEVO: Data/Repositories/Interfaces/ICategoriaRepository.cs
// ─────────────────────────────────────────────────────────────────────
using SenaPay.Domain.Entities;

namespace SenaPay.Domain.Interfaces;

    public interface ICategoriaRepository
    {
        Task<IEnumerable<Categoria>> ObtenerTodasAsync();
        Task<Categoria?> ObtenerPorIdAsync(int id);
        Task<IEnumerable<Categoria>> ObtenerPorIdsAsync(IEnumerable<int> ids);
        Task<bool> ExisteConNombreAsync(string nombre);
        Task AgregarAsync(Categoria categoria);
        Task EliminarAsync(Categoria categoria);
        Task<bool> ExisteConNombreExcluyendoIdAsync(string nombre, int idExcluir);
        Task GuardarCambiosAsync();
    }

