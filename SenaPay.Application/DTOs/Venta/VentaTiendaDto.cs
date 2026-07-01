namespace SenaPay.Application.DTOs.Venta;

public record VentaTiendaDto(
    int IdTransaccion,
    string NombreComprador,
    string Documento,
    DateTime Fecha,
    List<VentaItemDto> Items,
    decimal Total
);

public record VentaItemDto(
    string NombreProducto,
    int Cantidad,
    decimal PrecioUnitario
);
