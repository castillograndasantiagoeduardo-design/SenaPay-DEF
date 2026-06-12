using Microsoft.EntityFrameworkCore;
using SenaPay.Domain.Entities;
using SenaPay.Domain.Interfaces.Tienda;
using SenaPay.Infrastructure.Data;

namespace SenaPay.Infrastructure.Repositories.Tienda;

    /// <summary>
    /// Implementación concreta del repositorio de productos.
    /// Aquí está el único lugar donde escribimos LINQ / EF Core para Productos.
    /// </summary>
    /// 
    public class ProductoRepository : IProductoRepository
    {
        public async Task<IEnumerable<Producto>> ObtenerTodosAdminAsync()
        {
            return await _context.Productos
                .Include(p => p.IdCategoriaNavigation)
                .OrderBy(p => p.NombreProducto)
                .ToListAsync();
        }

        // Crear un producto nuevo
        public async Task CrearAsync(Producto producto)
        {
            producto.Estado = true;
            _context.Productos.Add(producto);
            await _context.SaveChangesAsync();
        }

        // Actualizar solo el stock (no toca precio ni nombre)
        public async Task ActualizarStockAsync(int idProducto, int nuevoStock)
        {
            var producto = await _context.Productos.FindAsync(idProducto);
            if (producto != null)
            {
                producto.Stock = nuevoStock;
                await _context.SaveChangesAsync();
            }
        }

        // Activar o desactivar (soft delete recomendado)
        public async Task ActivarDesactivarAsync(int idProducto, bool estado)
        {
            var producto = await _context.Productos.FindAsync(idProducto);
            if (producto != null)
            {
                producto.Estado = estado;
                await _context.SaveChangesAsync();
            }
        }

        // Eliminar permanente (úsalo con cuidado si hay transacciones relacionadas)
        public async Task EliminarAsync(int idProducto)
        {
            var producto = await _context.Productos.FindAsync(idProducto);
            if (producto != null)
            {
                _context.Productos.Remove(producto);
                await _context.SaveChangesAsync();
            }
        }

        private readonly SenaPayContext _context;

        // El DbContext se inyecta automáticamente por el contenedor de DI
        public ProductoRepository(SenaPayContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Producto>> ObtenerTodosActivosAsync()
        {
            return await _context.Productos
                .Include(p => p.IdCategoriaNavigation)    // Trae también la categoría (JOIN)
                .Include(p => p.IdTiendaNavigation)       // Trae también la tienda (JOIN)
                .Where(p => p.Estado == true && p.Stock > 0)
                .OrderBy(p => p.NombreProducto)
                .ToListAsync();
        }

        public async Task<IEnumerable<Producto>> ObtenerPorTiendaAsync(int idTienda)
        {
            return await _context.Productos
                .Include(p => p.IdCategoriaNavigation)
                .Where(p => p.IdTienda == idTienda && p.Estado == true)
                .ToListAsync();
        }

        public async Task<IEnumerable<Producto>> ObtenerPorCategoriaAsync(int idCategoria)
        {
            return await _context.Productos
                .Include(p => p.IdTiendaNavigation)
                .Where(p => p.IdCategoria == idCategoria && p.Estado == true)
                .ToListAsync();
        }

        public async Task<Producto?> ObtenerPorIdAsync(int idProducto)
        {
            return await _context.Productos
                .Include(p => p.IdCategoriaNavigation)
                .Include(p => p.IdTiendaNavigation)
                .FirstOrDefaultAsync(p => p.IdProducto == idProducto);
        }

        public async Task<bool> ReducirStockAsync(int idProducto, int cantidad)
        {
            var producto = await _context.Productos.FindAsync(idProducto);
            if (producto == null || producto.Stock < cantidad) return false;

            producto.Stock -= cantidad;

            // Si el stock llega a 0, desactivamos el producto automáticamente
            if (producto.Stock == 0)
                producto.Estado = false;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> TieneStockSuficienteAsync(int idProducto, int cantidad)
        {
            var producto = await _context.Productos.FindAsync(idProducto);
            return producto != null && producto.Stock >= cantidad && producto.Estado;
        }
    }



