namespace SenaPay.Application.UseCases.Tienda.DTOs;

/// <summary>
/// ViewModel empaquetado que la vista recibe listo para renderizar.
/// No contiene lógica — solo datos ya calculados por el caso de uso.
/// La vista no necesita hacer ningún .Where(), .Select() ni .Distinct().
/// </summary>
public class TiendaViewModel
{
    // ── Productos ya filtrados y mapeados ─────────────────────────
    public List<ProductoCardDto> Productos { get; init; } = new();

    // ── Categorías únicas listas para los chips HTML ──────────────
    public List<string> Categorias { get; init; } = new();

    // ── Totales para badges y estadísticas ───────────────────────
    public int TotalProductos { get; init; }
    public int ItemsEnCarrito { get; init; }

    // ── Filtros activos (para que la vista sepa cuál chip resaltar) ─
    public string? CategoriaActiva { get; init; }
    public string? BusquedaActiva { get; init; }
}

/// <summary>
/// Datos de un producto ya mapeados para la tarjeta visual.
/// La vista solo lee propiedades — sin navegar relaciones ni calcular nada.
/// </summary>
public class ProductoCardDto
{
    public int IdProducto { get; init; }
    public string NombreProducto { get; init; } = string.Empty;
    public decimal Precio { get; init; }
    public int Stock { get; init; }
    public bool Disponible { get; init; }    // Estado && Stock > 0

    // Navegación ya resuelta — la vista no toca IdCategoriaNavigation
    public string NombreCategoria { get; init; } = string.Empty;
    public string NombreTienda { get; init; } = string.Empty;

    // Stock calculado en el caso de uso, no en la vista
    public string StockCssClass { get; init; } = "stock-ok";
    public string StockIconCss { get; init; } = "bi-check-circle-fill";
    public string StockLabel { get; init; } = string.Empty;
    public string BtnLabel { get; init; } = "Agregar";
}