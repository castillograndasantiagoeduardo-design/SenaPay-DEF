using SenaPay.Application.UseCases.Account.DTOs;
using SenaPay.Domain.Interfaces.Usuarios;

namespace SenaPay.Application.UseCases.Account;

public class RestablecerPasswordUseCase
{
    private readonly IAccountRepository _repo;
    public RestablecerPasswordUseCase(IAccountRepository repo) => _repo = repo;

    public async Task<RestablecerPasswordResultado> EjecutarAsync(RestablecerPasswordRequest request)
    {
        if (request.NuevaPassword != request.ConfirmarPassword)
            return new RestablecerPasswordResultado(false, "Las contraseñas no coinciden.");

        var recuperacion = await _repo.ObtenerRecuperacionActivaAsync(request.Token);

        if (recuperacion?.IdUsuarioNavigation is null)
            return new RestablecerPasswordResultado(false, "El enlace de recuperación es inválido o ha expirado.");

        // ── Regla de negocio: actualizar clave e invalidar token ──
        recuperacion.IdUsuarioNavigation.Clave = request.NuevaPassword;
        recuperacion.Usado = true;

        await _repo.GuardarCambiosAsync();
        return new RestablecerPasswordResultado(true, "Tu contraseña ha sido actualizada. Ya puedes iniciar sesión.");
    }
}