using System.Collections.Generic;
using System.Threading.Tasks;
using SenaPay.Domain.Entities;



namespace SenaPay.Domain.Interfaces;

    public interface IProductoRepository
    {
        // ── Métodos para Clientes / Tienda ──────────────────────────────
        Task<IEnumerable<Producto>> ObtenerTodosActivosAsync();
        Task<IEnumerable<Producto>> ObtenerPorTiendaAsync(int idTienda);
        Task<IEnumerable<Producto>> ObtenerPorCategoriaAsync(int idTienda, int idCategoria);
        Task<Producto?> ObtenerPorIdAsync(int idProducto);
        Task<int> ContarBajoStockAsync(int idTienda, int umbral = 5);
        Task AgregarAsync(Producto producto);
        Task ActualizarAsync(Producto producto);
        Task EliminarAsync(int id);

    // ── Métodos para Administración ────────────────────────────────
    //Task<IEnumerable<Producto>> ObtenerTodosAdminAsync(); // Incluye inactivos
    //Task CrearAsync(Producto producto);
    //Task ActualizarStockAsync(int idProducto, int nuevoStock);
    //Task ActivarDesactivarAsync(int idProducto, bool activo);
    //Task EliminarAsync(int idProducto);
}
