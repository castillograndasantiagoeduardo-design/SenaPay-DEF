using SenaPay.Domain.Interfaces.Usuarios;

namespace SenaPay.Application.UseCases.Aprendiz;

/// <summary>
/// Caso de uso: Actualizar correo y teléfono del aprendiz.
/// Sin lógica de infraestructura, solo reglas de negocio.
/// </summary>
public class ActualizarPerfilAprendizUseCase
{
    private readonly IAprendizRepository _aprendizRepo;

    public ActualizarPerfilAprendizUseCase(IAprendizRepository aprendizRepo)
    {
        _aprendizRepo = aprendizRepo;
    }

    /// <summary>
    /// Ejecuta la actualización del perfil.
    /// </summary>
    /// <param name="documento">Documento del aprendiz en sesión.</param>
    /// <param name="dto">Datos nuevos a guardar.</param>
    /// <returns>true si se guardó correctamente, false si no existe el aprendiz.</returns>
    public async Task<bool> EjecutarAsync(int documento, ActualizarPerfilDto dto)
    {
        return await _aprendizRepo.ActualizarPerfilAsync(
            documento,
            dto.Correo.Trim(),
            string.IsNullOrWhiteSpace(dto.Telefono) ? null : dto.Telefono.Trim()
        );
    }
}
