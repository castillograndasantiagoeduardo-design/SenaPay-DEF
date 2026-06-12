using SenaPay.Application.UseCases.Account.DTOs;
using SenaPay.Domain.Interfaces.Usuarios;

namespace SenaPay.Application.UseCases.Account;

public class ValidarAccesoUseCase
{
    private readonly IAccountRepository _repo;
    public ValidarAccesoUseCase(IAccountRepository repo) => _repo = repo;

    public async Task<ValidarAccesoResultado> EjecutarAsync(ValidarAccesoRequest request)
    {
        if (request.IdRol == 3)
            return new ValidarAccesoResultado(false, "La vista de Administrador está en desarrollo.", 0, string.Empty);

        var usuario = await _repo.ObtenerUsuarioPorDocumentoYClaveAsync(
            request.Documento, request.Password);

        if (usuario is null)
            return new ValidarAccesoResultado(false, "Documento o contraseña incorrectos.", 0, string.Empty);

        if (usuario.IdRol != request.IdRol)
            return new ValidarAccesoResultado(false, "El rol seleccionado no coincide con su cuenta.", 0, string.Empty);

        return new ValidarAccesoResultado(true, string.Empty, usuario.IdRol, usuario.Documento.ToString());
    }
}