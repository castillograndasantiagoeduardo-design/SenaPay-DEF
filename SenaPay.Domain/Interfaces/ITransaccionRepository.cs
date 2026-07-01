using SenaPay.Domain.Entities;

namespace SenaPay.Domain.Interfaces;

public interface ITransaccionRepository
{
    Task<Transaccione> CrearAsync(
        int idAprendiz,
        decimal total,
        DateTime fecha,
        List<(int IdProducto, int Cantidad, decimal PrecioUnitario)> detalles);

    Task<int> CrearCompraAtomicaAsync(
        int idAprendiz,
        int idUsuario,
        decimal saldoActual,
        decimal total,
        DateTime fecha,
        List<(int IdProducto, int Cantidad, decimal PrecioUnitario)> detalles,
        bool esReserva,
        int idTienda);

    Task<List<Transaccione>> ObtenerPorTiendaAsync(int idTienda);
    Task<List<Transaccione>> ObtenerReservasPorTiendaAsync(int idTienda);
    Task CambiarEstadoReservaAsync(int idTransaccion, string nuevoEstado, int idTienda);
    Task<decimal> ObtenerTotalVentasHoyAsync(int idTienda);
    Task<int> ObtenerConteoTransaccionesHoyAsync(int idTienda);
}
