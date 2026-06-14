namespace SenaPay.Application.DTOs.Reportes;

public sealed record ReporteItemDto(
    int IdReporte,
    string Radicado,
    string TipoReporte,
    string Descripcion,
    string Estado,
    string FechaCreacion,
    string DocumentoUsuario,
    string? EvidenciaPath
);