using SenaPay.Domain.Entities;
using SenaPay.Domain.ValueObjects;

namespace SenaPay.Domain.Interfaces.Usuarios;

public interface IReporteRepository
{
    /// <summary>Persiste un nuevo reporte y devuelve true si tuvo éxito.</summary>
    Task<bool> CrearReporteAsync(Reporte reporte);

    /// <summary>Obtiene el Id_Usuario a partir de su número de documento.</summary>
    Task<int?> ObtenerIdUsuarioPorDocumentoAsync(int documento);
    // ── Nuevos métodos para el panel ──────────────────────────
    Task<IEnumerable<Reporte>> ObtenerTodosAsync();
    Task<Reporte?> ObtenerPorIdAsync(int idReporte);
    Task<bool> CambiarEstadoAsync(int idReporte, string nuevoEstado);
    Task<ReporteEstadisticas> ObtenerEstadisticasAsync();
}