namespace SenaPay.Domain.ValueObjects;

/// <summary>
/// Value Object de solo lectura.
/// Representa un snapshot de las estadísticas de reportes calculadas en tiempo real.
/// No se persiste — se deriva del estado actual de la tabla Reportes.
/// </summary>
public sealed class ReporteEstadisticas
{
    public int Total { get; }
    public int Pendientes { get; }
    public int EnRevision { get; }
    public int Resueltos { get; }

    // Constructor privado — solo se crea desde el método de fábrica
    private ReporteEstadisticas(int total, int pendientes, int enRevision, int resueltos)
    {
        Total = total;
        Pendientes = pendientes;
        EnRevision = enRevision;
        Resueltos = resueltos;
    }

    /// <summary>Crea las estadísticas a partir de los datos calculados.</summary>
    public static ReporteEstadisticas Crear(
        int total, int pendientes, int enRevision, int resueltos)
        => new(total, pendientes, enRevision, resueltos);

    /// <summary>Instancia vacía para cuando no hay reportes.</summary>
    public static ReporteEstadisticas Vacio()
        => new(0, 0, 0, 0);
}