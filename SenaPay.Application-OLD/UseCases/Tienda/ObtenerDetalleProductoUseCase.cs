using SenaPay.Domain.Entities;
using SenaPay.Domain.Interfaces.Tienda;

namespace SenaPay.Application.UseCases.Tienda;

/// <summary>
/// Caso de uso: Obtener el detalle completo de un producto por su ID.
/// Retorna null si el producto no existe.
/// </summary>
public class ObtenerDetalleProductoUseCase
{
    private readonly IProductoRepository _productoRepo;

    public ObtenerDetalleProductoUseCase(IProductoRepository productoRepo)
    {
        _productoRepo = productoRepo;
    }

    public async Task<Producto?> EjecutarAsync(int idProducto)
    {
        return await _productoRepo.ObtenerPorIdAsync(idProducto);
    }
}