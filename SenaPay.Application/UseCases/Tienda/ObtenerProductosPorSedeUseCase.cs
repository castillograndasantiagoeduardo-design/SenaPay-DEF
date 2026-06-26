using SenaPay.Application.DTOs.Tienda;
using SenaPay.Domain.Interfaces;

namespace SenaPay.Application.UseCases.Tienda;

public class ObtenerProductosPorSedeUseCase
{
    private readonly ITiendaRepository _tiendaRepo;
    private readonly IProductoRepository _productoRepo;

    public ObtenerProductosPorSedeUseCase(
        ITiendaRepository tiendaRepo,
        IProductoRepository productoRepo)
    {
        _tiendaRepo = tiendaRepo;
        _productoRepo = productoRepo;
    }

    /// <summary>
    /// Retorna el ViewModel de tienda filtrado por sede.
    /// Devuelve null si la sede no tiene tienda asignada.
    /// </summary>
    public async Task<(TiendaViewModel? vm, string? nombreTienda, int idTienda)> EjecutarAsync(
        int idSede,
        string? categoriaFiltro,
        string? busqueda,
        int itemsEnCarrito)
    {
        var tiendas = await _tiendaRepo.ObtenerPorSedeAsync(idSede);
        var tienda = tiendas.FirstOrDefault();

        if (tienda == null)
            return (null, null, 0);

        var todos = (await _productoRepo.ObtenerPorTiendaAsync(tienda.IdTienda))
            .Where(p => p.Estado)
            .ToList();

        var categorias = todos
            .Where(p => p.IdCategoriaNavigation != null)
            .Select(p => p.IdCategoriaNavigation!.Nombre_Categoria!)
            .Where(n => !string.IsNullOrEmpty(n))
            .Distinct()
            .OrderBy(c => c)
            .ToList();

        var filtrados = todos.AsEnumerable();

        if (!string.IsNullOrWhiteSpace(categoriaFiltro))
            filtrados = filtrados.Where(p =>
                p.IdCategoriaNavigation?.Nombre_Categoria == categoriaFiltro);

        if (!string.IsNullOrWhiteSpace(busqueda))
        {
            var q = busqueda.ToLower();
            filtrados = filtrados.Where(p =>
                (p.NombreProducto?.ToLower().Contains(q) ?? false) ||
                (p.IdCategoriaNavigation?.Nombre_Categoria?.ToLower().Contains(q) ?? false));
        }

        var productos = filtrados.Select(p =>
        {
            var (cssClass, iconCss, label) = p.Stock switch
            {
                0 => ("stock-out", "bi-x-circle-fill", "Sin stock"),
                <= 5 => ("stock-low", "bi-exclamation-circle-fill", $"Solo {p.Stock} restantes"),
                _ => ("stock-ok", "bi-check-circle-fill", $"{p.Stock} disponibles")
            };

            return new ProductoCardDto
            {
                IdProducto = p.IdProducto,
                IdTienda = tienda.IdTienda,
                NombreProducto = p.NombreProducto ?? string.Empty,
                Precio = p.Precio,
                Stock = p.Stock,
                Disponible = p.Estado && p.Stock > 0,
                Imagen = p.Imagen,
                NombreCategoria = p.IdCategoriaNavigation?.Nombre_Categoria ?? string.Empty,
                NombreTienda = tienda.Nombre ?? string.Empty,
                StockCssClass = cssClass,
                StockIconCss = iconCss,
                StockLabel = label,
                BtnLabel = p.Stock == 0 ? "Agotado" : "Agregar"
            };
        }).ToList();

        var vm = new TiendaViewModel
        {
            Productos = productos,
            Categorias = categorias,
            TotalProductos = todos.Count,
            ItemsEnCarrito = itemsEnCarrito,
            CategoriaActiva = categoriaFiltro,
            BusquedaActiva = busqueda
        };

        return (vm, tienda.Nombre, tienda.IdTienda);
    }
}
