using SenaPay.Domain.Entities;
using SenaPay.Domain.Interfaces.Usuarios;

namespace SenaPay.Application.UseCases.Account;

public sealed class CrearReporteUseCase
{
    private readonly IReporteRepository _reporteRepo;

    public CrearReporteUseCase(IReporteRepository reporteRepo)
    {
        _reporteRepo = reporteRepo;
    }

    /// <summary>
    /// Ejecuta el flujo: busca el Id_Usuario por documento,
    /// genera el radicado y persiste el reporte.
    /// </summary>
    /// <returns>El radicado generado, o null si el usuario no existe.</returns>
    public async Task<string?> EjecutarAsync(CrearReporteDto dto)
    {
        // 1. Resolver el Id_Usuario desde el documento enviado
        //Se valida que el documento que viene desde Vista/Controlador este en la BSD
        int? idUsuario = await _reporteRepo
            .ObtenerIdUsuarioPorDocumentoAsync(dto.Documento);

        if (idUsuario is null)
            return null; // El documento no corresponde a ningún usuario registrado

        // 2. Generar el número de radicado único
        string radicado = GenerarRadicado();

        // 3. Construir la entidad Reporte
        var reporte = new Reporte
        {
            Radicado = radicado,
            Tipo_Reporte = dto.TipoReporte,
            Descripcion = dto.Descripcion,
            Evidencia_Path = dto.EvidenciaPath,
            Estado = "Pendiente",
            Fecha_Creacion = DateTime.UtcNow,
            Id_Usuario = idUsuario.Value
        };

        // 4. Persistir
        //Se envia el reporte a el repo para que lo agregue a la BSD
        bool creado = await _reporteRepo.CrearReporteAsync(reporte);
         //Aca le enviamos la confirmacion al controlador para que se la de a la vista
         //Si creado es true return radicado y  si no retorna null
        return creado ? radicado : null;
    }

    // ── Helpers ────────────────────────────────────────────────────────────
    private static string GenerarRadicado()
    {
        int anio = DateTime.UtcNow.Year;
        string random = Random.Shared.Next(10000, 99999).ToString();
        return $"SP-{anio}-{random}";
    }
}