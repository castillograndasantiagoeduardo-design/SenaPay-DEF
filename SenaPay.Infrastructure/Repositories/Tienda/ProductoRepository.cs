using Microsoft.EntityFrameworkCore;
using SenaPay.Domain.Entities;
using SenaPay.Domain.Interfaces;
using SenaPay.Infrastructure.Data;

namespace SenaPay.Infrastructure.Repositories.Tienda;

/// <summary>
/// Implementación concreta del repositorio de productos.
/// Único lugar donde se escribe LINQ / EF Core para Productos.
/// </summary>
public class ProductoRepository : IProductoRepository
{
    private readonly SenaPayContext _context;

    public ProductoRepository(SenaPayContext context)
    {
        _context = context;
    }

    // ── Clientes / Tienda ────────────────────────────────────────

    public async Task<IEnumerable<Producto>> ObtenerTodosActivosAsync()
    {
        return await _context.Productos
            .Include(p => p.IdCategoriaNavigation)
            .Include(p => p.IdTiendaNavigation)
            .Where(p => p.Estado == true && p.Stock > 0)
            .OrderBy(p => p.NombreProducto)
            .ToListAsync();
    }

    public async Task<IEnumerable<Producto>> ObtenerPorTiendaAsync(int idTienda)
    {
        return await _context.Productos
            .Include(p => p.IdCategoriaNavigation)
            .Where(p => p.IdTienda == idTienda)
            .OrderBy(p => p.NombreProducto)
            .ToListAsync();
    }

    // ✅ Firma corregida: dos parámetros, igual que la interfaz
    public async Task<IEnumerable<Producto>> ObtenerPorCategoriaAsync(int idTienda, int idCategoria)
    {
        return await _context.Productos
            .Include(p => p.IdCategoriaNavigation)
            .Where(p => p.IdTienda == idTienda && p.IdCategoria == idCategoria)
            .OrderBy(p => p.NombreProducto)
            .ToListAsync();
    }

    public async Task<Producto?> ObtenerPorIdAsync(int idProducto)
    {
        return await _context.Productos
            .Include(p => p.IdCategoriaNavigation)
            .Include(p => p.IdTiendaNavigation)
            .FirstOrDefaultAsync(p => p.IdProducto == idProducto);
    }

    public async Task<int> ContarBajoStockAsync(int idTienda, int umbral = 5)
    {
        return await _context.Productos
            .CountAsync(p => p.IdTienda == idTienda && p.Stock <= umbral && p.Estado == true);
    }

    // ── CRUD ─────────────────────────────────────────────────────

    public async Task AgregarAsync(Producto producto)
    {
        producto.Estado = true;
        _context.Productos.Add(producto);
        await _context.SaveChangesAsync();
    }

    public async Task ActualizarAsync(Producto producto)
    {
        _context.Productos.Update(producto);
        await _context.SaveChangesAsync();
    }

    public async Task EliminarAsync(int id)
    {
        var producto = await _context.Productos.FindAsync(id);
        if (producto != null)
        {
            _context.Productos.Remove(producto);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<Producto?> ObtenerPorCodigoBarrasAsync(string codigo)
        => await _context.Productos
            .Include(p => p.IdCategoriaNavigation)
            .Include(p => p.IdTiendaNavigation)
            .FirstOrDefaultAsync(p => p.CodigoBarras == codigo);

    public async Task<bool> CodigoBarrasExisteAsync(string codigo, int? excluirId = null)
        => await _context.Productos.AnyAsync(p =>
            p.CodigoBarras == codigo &&
            (!excluirId.HasValue || p.IdProducto != excluirId.Value));

    // ── Stock ─────────────────────────────────────────────────────

    public async Task<bool> ReducirStockAsync(int idProducto, int cantidad)
    {
        var producto = await _context.Productos.FindAsync(idProducto);
        if (producto == null || producto.Stock < cantidad) return false;

        producto.Stock -= cantidad;

        if (producto.Stock == 0)
            producto.Estado = false;

        await _context.SaveChangesAsync();
        return true;
    }

    // ── Admin global ──────────────────────────────────────────────

    public async Task<IEnumerable<Producto>> ObtenerTodosAdminAsync()
    {
        return await _context.Productos
            .Include(p => p.IdCategoriaNavigation)
            .OrderBy(p => p.NombreProducto)
            .ToListAsync();
    }
}