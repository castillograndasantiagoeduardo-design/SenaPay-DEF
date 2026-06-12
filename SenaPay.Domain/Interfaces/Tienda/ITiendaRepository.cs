using SenaPay.Domain.Entities;

namespace SenaPay.Domain.Interfaces.Tienda;

public interface ITiendaRepository
{
    Task<IEnumerable<Tiendum>> ObtenerTodasAsync();
    Task<IEnumerable<Tiendum>> ObtenerPorSedeAsync(int idSede);
    Task<Tiendum?> ObtenerPorIdAsync(int idTienda);
    Task<Tiendum?> ObtenerPorAdminAsync(int idUsuario);
    Task CrearAsync(Tiendum tienda);
    Task GuardarCambiosAsync();
}