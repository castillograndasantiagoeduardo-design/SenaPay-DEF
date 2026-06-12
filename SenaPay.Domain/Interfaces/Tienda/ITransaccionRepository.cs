using SenaPay.Domain.Entities;

namespace SenaPay.Domain.Interfaces.Tienda;

    public interface ITransaccionRepository
    {
        Task<Transaccione> CrearTransaccionAsync(Transaccione transaccion);
        Task<IEnumerable<Transaccione>> ObtenerHistorialAprendizAsync(int idAprendiz);
        Task<Transaccione?> ObtenerConDetallesAsync(int idTransaccion);
    }


