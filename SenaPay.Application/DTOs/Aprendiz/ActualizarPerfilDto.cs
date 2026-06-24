namespace SenaPay.Application.DTOs.Aprendiz;

/// <summary>
/// Datos que llegan desde la vista para actualizar el perfil del aprendiz.
/// </summary>
public record ActualizarPerfilDto(
    string Correo,
    string? Telefono
);
