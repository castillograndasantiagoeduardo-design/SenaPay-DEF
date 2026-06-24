namespace SenaPay.Application.DTOs.Aprendiz;

/// <summary>
/// Datos que el caso de uso devuelve hacia el controlador.
/// Solo expone lo que la vista necesita, sin exponer la entidad de dominio.
/// </summary>
public record PerfilAprendizDto(
    int IdAprendiz,
    string Nombre,
    decimal Saldo,
    string Correo,
    string? Telefono,
    string Ficha,
    string Documento,
    string Sede,
    List<TransaccionResumenDto> UltimasTransacciones
);

/// <summary>
/// Resumen de una transacción para mostrar en el historial del dashboard.
/// </summary>
public record TransaccionResumenDto(
    int IdTransaccion,
    decimal Total,
    DateTime Fecha,
    string Descripcion   // nombre del primer producto o "Compra en tienda"
);