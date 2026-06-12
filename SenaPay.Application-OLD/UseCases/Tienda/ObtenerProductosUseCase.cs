using SenaPay.Application.UseCases.Tienda.DTOs;
using SenaPay.Domain.Interfaces.Tienda;

namespace SenaPay.Application.UseCases.Tienda;

/// <summary>
/// Caso de uso: Obtener el ViewModel completo para la página de tienda.
///
/// Toda la lógica que antes vivía en el HTML de la vista ahora vive aquí:
/// - Filtrado por categoría y texto
/// - Extracción de categorías únicas para los chips
/// - Cálculo de clases CSS de stock
/// - Mapeo de entidades a DTOs
/// </summary>
public class ObtenerProductosUseCase
{
    private readonly IProductoRepository _productoRepo;

    public ObtenerProductosUseCase(IProductoRepository productoRepo)
    {
        _productoRepo = productoRepo;
    }

    /// <param name="categoriaFiltro">Nombre de categoría seleccionada (null = todas).</param>
    /// <param name="busqueda">Texto de búsqueda libre (null = sin filtro).</param>
    /// <param name="itemsEnCarrito">Cantidad de ítems en el carrito (viene de Session).</param>
    public async Task<TiendaViewModel> EjecutarAsync(
        string? categoriaFiltro,
        string? busqueda,
        int itemsEnCarrito)
    {
        // ── 1. Traer todos los productos activos con sus navegaciones ─
        var todos = (await _productoRepo.ObtenerTodosActivosAsync()).ToList();

        // ── 2. Extraer categorías únicas ANTES de filtrar ────────────
        //    (los chips muestran todas las categorías, no solo las filtradas)
        var categorias = todos
            .Where(p => p.IdCategoriaNavigation != null)
            .Select(p => p.IdCategoriaNavigation!.Nombre_Categoria)
            .Distinct()
            .OrderBy(c => c)
            .ToList();

        // ── 3. Aplicar filtros ────────────────────────────────────────
        var filtrados = todos.AsEnumerable();

        if (!string.IsNullOrWhiteSpace(categoriaFiltro))
            filtrados = filtrados.Where(p =>
                p.IdCategoriaNavigation?.Nombre_Categoria == categoriaFiltro);

        if (!string.IsNullOrWhiteSpace(busqueda))
        {
            var q = busqueda.ToLower();
            filtrados = filtrados.Where(p =>
                (p.NombreProducto?.ToLower().Contains(q) ?? false) ||
                (p.IdCategoriaNavigation?.Nombre_Categoria.ToLower().Contains(q) ?? false) ||
                (p.IdTiendaNavigation?.Nombre?.ToLower().Contains(q) ?? false));
        }

        // ── 4. Mapear a DTOs de presentación ─────────────────────────
        var productos = filtrados.Select(p => MapearProducto(p)).ToList();

        return new TiendaViewModel
        {
            Productos = productos,
            Categorias = categorias,
            TotalProductos = todos.Count,
            ItemsEnCarrito = itemsEnCarrito,
            CategoriaActiva = categoriaFiltro,
            BusquedaActiva = busqueda
        };
    }

    // ── Mapeo de entidad → DTO (lógica de presentación centralizada) ─
    private static ProductoCardDto MapearProducto(SenaPay.Domain.Entities.Producto p)
    {
        // Calcular clases y etiquetas de stock aquí, no en la vista
        var (cssClass, iconCss, label) = p.Stock switch
        {
            0 => ("stock-out", "bi-x-circle-fill", "Sin stock"),
            <= 5 => ("stock-low", "bi-exclamation-circle-fill", $"Solo {p.Stock} restantes"),
            _ => ("stock-ok", "bi-check-circle-fill", $"{p.Stock} disponibles")
        };

        return new ProductoCardDto
        {
            IdProducto = p.IdProducto,
            NombreProducto = p.NombreProducto ?? string.Empty,
            Precio = p.Precio,
            Stock = p.Stock,
            Disponible = p.Estado && p.Stock > 0,
            NombreCategoria = p.IdCategoriaNavigation?.Nombre_Categoria ?? string.Empty,
            NombreTienda = p.IdTiendaNavigation?.Nombre ?? string.Empty,
            StockCssClass = cssClass,
            StockIconCss = iconCss,
            StockLabel = label,
            BtnLabel = p.Stock == 0 ? "Agotado" : "Agregar"
        };
    }
}