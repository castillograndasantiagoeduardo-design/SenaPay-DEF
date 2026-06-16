using SenaPay.Domain.Interfaces.Usuarios;

namespace SenaPay.Application.UseCases.Aprendiz;

/// <summary>
/// Caso de uso: Obtener el perfil completo de un aprendiz a partir de su documento.
/// Contiene la lógica de negocio; no depende de EF Core ni de HttpContext.
/// </summary>
public class GetPerfilAprendizUseCase
{
    private readonly IAprendizRepository _aprendizRepo;

    public GetPerfilAprendizUseCase(IAprendizRepository aprendizRepo)
    {
        _aprendizRepo = aprendizRepo;
    }

    /// <summary>
    /// Ejecuta el caso de uso.
    /// </summary>
    /// <param name="documento">Número de documento del aprendiz.</param>
    /// <returns>DTO con datos del perfil completo, o null si no existe.</returns>
    public async Task<PerfilAprendizDto?> EjecutarAsync(int documento)
    {
        var aprendiz = await _aprendizRepo.ObtenerPerfilCompletoAsync(documento);

        if (aprendiz is null) return null;

        var usuario = aprendiz.IdUsuarioNavigation;
        var sede = usuario?.IdSedeNavigation;

        // ── Mapear transacciones ──────────────────────────────────────────
        var transacciones = aprendiz.Transacciones
            .OrderByDescending(t => t.Fecha)
            .Select(t => new TransaccionResumenDto(
                IdTransaccion: t.IdTransaccion,
                Total: t.Total,
                Fecha: t.Fecha,
                // Toma el nombre del primer producto del detalle, o texto genérico
                Descripcion: t.DetalleTransaccions
                                .FirstOrDefault()
                                ?.IdProductoNavigation
                                ?.NombreProducto
                                ?? "Compra en tienda"
            ))
            .ToList();

        return new PerfilAprendizDto(
            IdAprendiz: aprendiz.IdAprendiz,
            Nombre: aprendiz.Nombre,
            Saldo: aprendiz.Saldo,
            Correo: aprendiz.Correo,
            Telefono: aprendiz.Telefono,
            Ficha: "2827102",   // reemplazar con aprendiz.Ficha cuando exista en BD
            Documento: usuario?.Documento.ToString() ?? "—",
            Sede: sede?.Nombre ?? "Sin sede",
            UltimasTransacciones: transacciones
        );
    }
}