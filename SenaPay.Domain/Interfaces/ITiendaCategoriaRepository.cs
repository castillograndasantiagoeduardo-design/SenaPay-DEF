namespace SenaPay.Domain.Interfaces;

public interface ITiendaCategoriaRepository
{
    Task<IEnumerable<int>> ObtenerIdsHabilitadosPorTiendaAsync(int idTienda);
    Task HabilitarCategoriasAsync(int idTienda, IEnumerable<int> idsCategorias);
    Task DeshabilitarCategoriaAsync(int idTienda, int idCategoria);
    Task<bool> TieneCategoriasAsync(int idTienda);
}