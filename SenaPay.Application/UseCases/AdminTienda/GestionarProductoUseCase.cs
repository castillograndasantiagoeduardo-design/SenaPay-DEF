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
            CodigoBarras = p.CodigoBarras,
            Estado = p.Estado,
            IdCategoria = p.IdCategoria,
            NombreCategoria = p.IdCategoriaNavigation?.Nombre_Categoria
        });
    }

    public async Task<string?> CrearAsync(CrearProductoRequest r)
    {
        var codigo = string.IsNullOrWhiteSpace(r.CodigoBarras) ? null : r.CodigoBarras.Trim();
        if (codigo != null && await _repo.CodigoBarrasExisteAsync(codigo))
            return "Ya existe un producto con ese código de barras.";

        var producto = new Producto
        {
            NombreProducto = r.NombreProducto.Trim(),
            Precio = r.Precio,
            Stock = r.Stock,
            Imagen = r.Imagen,
            CodigoBarras = codigo,
            Estado = true,
            IdTienda = r.IdTienda,
            IdCategoria = r.IdCategoria
        };
        await _repo.AgregarAsync(producto);
        return null;
    }

    public async Task<string?> EditarAsync(EditarProductoRequest r)
    {
        var producto = await _repo.ObtenerPorIdAsync(r.IdProducto)
            ?? throw new Exception("Producto no encontrado.");

        var codigo = string.IsNullOrWhiteSpace(r.CodigoBarras) ? null : r.CodigoBarras.Trim();
        if (codigo != null && await _repo.CodigoBarrasExisteAsync(codigo, r.IdProducto))
            return "Ya existe un producto con ese código de barras.";

        producto.NombreProducto = r.NombreProducto.Trim();
        producto.Precio = r.Precio;
        producto.Stock = r.Stock;
        producto.Imagen = r.Imagen;
        producto.CodigoBarras = codigo;
        producto.Estado = r.Estado;
        producto.IdCategoria = r.IdCategoria;

        await _repo.ActualizarAsync(producto);
        return null;
    }

    public async Task EliminarAsync(int id)
        => await _repo.EliminarAsync(id);
    public async Task<int> ContarBajoStockAsync(int idTienda, int umbral = 5)
    {
        return await _repo.ContarBajoStockAsync(idTienda, umbral);
    }
}