namespace SenaPay.Application.DTOs.Carrito;

public class ItemCarritoDto
{
    public int IdProducto { get; set; }
    public int IdTienda { get; set; }
    public string NombreProducto { get; set; } = string.Empty;
    public decimal Precio { get; set; }
    public int Cantidad { get; set; }
    public string? Imagen { get; set; }
    public string NombreCategoria { get; set; } = string.Empty;
    public decimal Subtotal => Precio * Cantidad;
}

public class AgregarAlCarritoRequest
{
    public int IdProducto { get; set; }
    public int IdTienda { get; set; }
    public string NombreProducto { get; set; } = string.Empty;
    public decimal Precio { get; set; }
    public string? Imagen { get; set; }
    public string NombreCategoria { get; set; } = string.Empty;
}

public class ActualizarCarritoRequest
{
    public int IdProducto { get; set; }
    public int Delta { get; set; } // +1 o -1, o 0 para eliminar
}

public class PagarRequest
{
    public DateTime? FechaReserva { get; set; }
}

public class ResultadoPagoDto
{
    public bool Ok { get; set; }
    public string Mensaje { get; set; } = string.Empty;
    public decimal? SaldoRestante { get; set; }
    public int? IdTransaccion { get; set; }
    public bool EsReserva { get; set; }
    public string? FechaReservaFormateada { get; set; }
}
