namespace SenaPay.Application.UseCases.Funcionarios.DTOs;

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