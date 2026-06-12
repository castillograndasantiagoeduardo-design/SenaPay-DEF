using SenaPay.Domain.Entities;

namespace SenaPay.Domain.Interfaces.Usuarios;

/// <summary>
/// Contrato de autenticación, recuperación de contraseña y sesión.
/// </summary>
public interface IAccountRepository
{
    Task<Usuario?> ObtenerUsuarioPorDocumentoYClaveAsync(int documento, string clave);
    Task<(Usuario? usuario, string? correo)> ObtenerUsuarioConCorreoAsync(int documento);
    Task<RecuperacionPassword?> ObtenerRecuperacionActivaAsync(string token);
    Task AgregarRecuperacionAsync(RecuperacionPassword recuperacion);
    Task GuardarCambiosAsync();
}