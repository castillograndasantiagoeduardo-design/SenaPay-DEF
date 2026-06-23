using SenaPay.Domain.Entities;

namespace SenaPay.Domain.Interfaces;

public interface ISedeRepository
{
    Task<IEnumerable<Sede>> ObtenerTodasAsync();
    Task<Sede?> ObtenerPorIdAsync(int idSede);
}