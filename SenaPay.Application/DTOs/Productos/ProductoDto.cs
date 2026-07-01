namespace SenaPay.Application.DTOs.Producto;

public class ProductoDto
{
    public int IdProducto { get; set; }
    public string NombreProducto { get; set; } = null!;
    public decimal Precio { get; set; }
    public int Stock { get; set; }
    public string? Imagen { get; set; }
    public string? CodigoBarras { get; set; }
    public bool Estado { get; set; }
    public int? IdCategoria { get; set; }
    public string? NombreCategoria { get; set; }

    // Pre-computed para la vista (sin LINQ en Razor)
    public string StockCssClass => Stock <= 5 ? "badge-danger" : Stock <= 15 ? "badge-warning" : "badge-success";
    public string StockLabel => Stock <= 5 ? "Crítico" : Stock <= 15 ? "Bajo" : "OK";
    public string PrecioFormateado => Precio.ToString("C0");
}