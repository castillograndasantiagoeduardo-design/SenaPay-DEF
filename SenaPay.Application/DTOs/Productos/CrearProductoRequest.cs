namespace SenaPay.Application.DTOs.Producto;

public class CrearProductoRequest
{
    public string NombreProducto { get; set; } = null!;
    public decimal Precio { get; set; }
    public int Stock { get; set; }
    public string? Imagen { get; set; }
    public string? CodigoBarras { get; set; }
    public int? IdCategoria { get; set; }
    public int IdTienda { get; set; }
}