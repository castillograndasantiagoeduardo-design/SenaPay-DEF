using System.Collections.Generic;
using System.Threading.Tasks;
using SenaPay.Domain.Entities;



namespace SenaPay.Domain.Interfaces.Tienda;

    public interface IProductoRepository
    {
        // ── Métodos para Clientes / Tienda ──────────────────────────────
        Task<IEnumerable<Producto>> ObtenerTodosActivosAsync();
        Task<IEnumerable<Producto>> ObtenerPorTiendaAsync(int idTienda);
        Task<IEnumerable<Producto>> ObtenerPorCategoriaAsync(int idCategoria);
        Task<Producto?> ObtenerPorIdAsync(int idProducto);
        Task<bool> ReducirStockAsync(int idProducto, int cantidad);
        Task<bool> TieneStockSuficienteAsync(int idProducto, int cantidad);

        // ── Métodos para Administración ────────────────────────────────
        //Task<IEnumerable<Producto>> ObtenerTodosAdminAsync(); // Incluye inactivos
        //Task CrearAsync(Producto producto);
        //Task ActualizarStockAsync(int idProducto, int nuevoStock);
        //Task ActivarDesactivarAsync(int idProducto, bool activo);
        //Task EliminarAsync(int idProducto);
    }
