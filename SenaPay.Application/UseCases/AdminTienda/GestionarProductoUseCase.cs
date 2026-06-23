using SenaPay.Application.DTOs.Producto;
using SenaPay.Domain.Entities;
using SenaPay.Domain.Interfaces;

namespace SenaPay.Application.UseCases.AdminTienda;

public class GestionarProductoUseCase
{
    private readonly IProductoRepository _repo;

    public GestionarProductoUseCase(IProductoRepository repo)
        => _repo = repo;

    public async Task<IEnumerable<ProductoDto>> ObtenerInventarioAsync(int idTienda, int? idCategoria)
    {
        var productos = idCategoria.HasValue
            ? await _repo.ObtenerPorCategoriaAsync(idTienda, idCategoria.Value)
            : await _repo.ObtenerPorTiendaAsync(idTienda);

        return productos.Select(p => new ProductoDto
        {
            IdProducto = p.IdProducto,
            NombreProducto = p.NombreProducto,
            Precio = p.Precio,
            Stock = p.Stock,
            Imagen = p.Imagen,
            Estado = p.Estado,
            IdCategoria = p.IdCategoria,
            NombreCategoria = p.IdCategoriaNavigation?.Nombre_Categoria
        });
    }

    public async Task CrearAsync(CrearProductoRequest r)
    {
        var producto = new Producto
        {
            NombreProducto = r.NombreProducto.Trim(),
            Precio = r.Precio,
            Stock = r.Stock,
            Imagen = r.Imagen,
            Estado = true,
            IdTienda = r.IdTienda,
            IdCategoria = r.IdCategoria
        };
        await _repo.AgregarAsync(producto);
    }

    public async Task EditarAsync(EditarProductoRequest r)
    {
        var producto = await _repo.ObtenerPorIdAsync(r.IdProducto)
            ?? throw new Exception("Producto no encontrado.");

        producto.NombreProducto = r.NombreProducto.Trim();
        producto.Precio = r.Precio;
        producto.Stock = r.Stock;
        producto.Imagen = r.Imagen;
        producto.Estado = r.Estado;
        producto.IdCategoria = r.IdCategoria;

        await _repo.ActualizarAsync(producto);
    }

    public async Task EliminarAsync(int id)
        => await _repo.EliminarAsync(id);
    public async Task<int> ContarBajoStockAsync(int idTienda, int umbral = 5)
    {
        return await _repo.ContarBajoStockAsync(idTienda, umbral);
    }
}