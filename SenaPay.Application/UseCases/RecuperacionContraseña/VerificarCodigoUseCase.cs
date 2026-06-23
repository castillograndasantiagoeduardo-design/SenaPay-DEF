using SenaPay.Application.DTOs.Usuarios;
using SenaPay.Domain.Interfaces;

namespace SenaPay.Application.UseCases.RecuperacionContraseña;

public class VerificarCodigoUseCase
{
    private readonly IAccountRepository _repo;
    public VerificarCodigoUseCase(IAccountRepository repo) => _repo = repo;

    public async Task<VerificarCodigoResultado> EjecutarAsync(string codigo)
    {
        var recuperacion = await _repo.ObtenerRecuperacionActivaAsync(codigo);

        return recuperacion is not null
            ? new VerificarCodigoResultado(true, codigo)
            : new VerificarCodigoResultado(false, string.Empty);
    }
}