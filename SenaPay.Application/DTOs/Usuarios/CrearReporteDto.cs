namespace SenaPay.Application.DTOs.Usuarios;

public sealed record CrearReporteDto(
    string TipoDocumento,
    int Documento,
    string Rol,
    string TipoReporte,
    string Descripcion,
    string? EvidenciaPath   // ruta guardada previamente si se subió archivo
);