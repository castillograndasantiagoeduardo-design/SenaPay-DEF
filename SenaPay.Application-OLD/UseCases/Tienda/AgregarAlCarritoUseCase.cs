using SenaPay.Domain.Interfaces.Tienda.DTOs;
using SenaPay.Domain.Interfaces.Tienda;

namespace SenaPay.Application.UseCases.Tienda;

/// <summary>
/// Caso de uso: Agregar un producto al carrito.
/// 
/// Responsabilidades:
/// - Verificar que el producto existe y está activo.
/// - Verificar que hay stock suficiente.
/// - Retornar el carrito actualizado o un mensaje de error.
/// 
/// NO toca HttpContext ni Session — eso es responsabilidad del controlador.
/// </summary>
public class AgregarAlCarritoUseCase
{
    private readonly IProductoRepository _productoRepo;

    public AgregarAlCarritoUseCase(IProductoRepository productoRepo)
    {
        _productoRepo = productoRepo;
    }

    /// <summary>
    /// Agrega o incrementa un producto en el carrito.
    /// </summary>
    /// <param name="carritoActual">Estado actual del carrito (leído de sesión por el controlador).</param>
    /// <param name="idProducto">Producto a agregar.</param>
    /// <param name="cantidad">Unidades a agregar.</param>
    /// <returns>El carrito modificado y un mensaje de resultado.</returns>
    public async Task<(CarritoDto Carrito, bool Ok, string Mensaje)> EjecutarAsync(
        CarritoDto carritoActual,
        int idProducto,
        int cantidad = 1)
    {
        // ── Regla 1: el producto debe existir ────────────────────────
        var producto = await _productoRepo.ObtenerPorIdAsync(idProducto);
        if (producto is null)
            return (carritoActual, false, "El producto no existe.");

        // ── Regla 2: el producto debe estar activo ───────────────────
        if (!producto.Estado)
            return (carritoActual, false, "El producto no está disponible.");

        // ── Regla 3: calcular cuántas unidades ya hay en el carrito ──
        var itemExistente = carritoActual.Items
            .FirstOrDefault(i => i.Id_Producto == idProducto);

        int cantidadTotalPedida = (itemExistente?.Cantidad ?? 0) + cantidad;

        // ── Regla 4: verificar stock suficiente ──────────────────────
        if (!await _productoRepo.TieneStockSuficienteAsync(idProducto, cantidadTotalPedida))
            return (carritoActual, false,
                $"Stock insuficiente. Solo hay {producto.Stock} unidades disponibles.");

        // ── Actualizar el carrito ────────────────────────────────────
        if (itemExistente is not null)
        {
            itemExistente.Cantidad += cantidad;
        }
        else
        {
            carritoActual.Items.Add(new ItemCarritoDto
            {
                Id_Producto = producto.IdProducto,
                NombreProducto = producto.NombreProducto,
                PrecioUnitario = producto.Precio,
                Cantidad = cantidad
            });
        }

        return (carritoActual, true,
            $"'{producto.NombreProducto}' agregado al carrito.");
    }
}