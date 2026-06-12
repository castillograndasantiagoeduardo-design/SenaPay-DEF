using SenaPay.Application.UseCases.Account.DTOs;
using SenaPay.Domain.Interfaces.Usuarios;

namespace SenaPay.Application.UseCases.Account;

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