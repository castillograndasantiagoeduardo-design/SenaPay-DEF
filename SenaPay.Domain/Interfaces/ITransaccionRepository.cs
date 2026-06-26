using SenaPay.Domain.Entities;

namespace SenaPay.Domain.Interfaces;

public interface ITransaccionRepository
{
    Task<Transaccione> CrearAsync(
        int idAprendiz,
        decimal total,
        DateTime fecha,
        List<(int IdProducto, int Cantidad, decimal PrecioUnitario)> detalles);
}
