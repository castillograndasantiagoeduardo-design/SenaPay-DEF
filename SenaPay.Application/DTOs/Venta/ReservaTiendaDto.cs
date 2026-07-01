namespace SenaPay.Application.DTOs.Venta;

public record ReservaTiendaDto(
    int IdTransaccion,
    string NombreComprador,
    string Documento,
    DateTime FechaCreacion,
    DateTime? FechaRetiro,
    string EstadoReserva,
    List<VentaItemDto> Items,
    decimal Total
);
