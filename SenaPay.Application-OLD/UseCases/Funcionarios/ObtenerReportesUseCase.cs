using SenaPay.Application.UseCases.Funcionarios.DTOs;
using SenaPay.Domain.Interfaces.Usuarios;

namespace SenaPay.Application.UseCases.Funcionarios;

public sealed class ObtenerReportesUseCase
{
    private readonly IReporteRepository _repo;
    public ObtenerReportesUseCase(IReporteRepository repo) => _repo = repo;

    public async Task<IEnumerable<ReporteItemDto>> EjecutarAsync()
    {
        var reportes = await _repo.ObtenerTodosAsync();
        return reportes.Select(r => new ReporteItemDto(
            IdReporte: r.Id_Reporte,
            Radicado: r.Radicado,
            TipoReporte: r.Tipo_Reporte,
            Descripcion: r.Descripcion,
            Estado: r.Estado,
            FechaCreacion: r.Fecha_Creacion
                              .ToString("dd/MM/yyyy HH:mm"),
            DocumentoUsuario: r.Usuario?.Documento.ToString() ?? "—",
            EvidenciaPath: r.Evidencia_Path
        ));
    }
}