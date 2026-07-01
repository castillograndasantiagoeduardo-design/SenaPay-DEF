using SenaPay.Application.DTOs.Funcionario;
using SenaPay.Domain.Interfaces;

namespace SenaPay.Application.UseCases.Tienda;

public class GetPerfilFuncionarioUseCase
{
    private readonly IFuncionarioRepository _repo;

    public GetPerfilFuncionarioUseCase(IFuncionarioRepository repo)
    {
        _repo = repo;
    }

    public async Task<PerfilFuncionarioDto?> EjecutarAsync(int documento)
    {
        var funcionario = await _repo.ObtenerPorDocumentoAsync(documento);
        if (funcionario is null) return null;

        var usuario = funcionario.IdUsuarioNavigation;
        var sede    = usuario?.IdSedeNavigation;

        return new PerfilFuncionarioDto(
            IdFuncionario: funcionario.IdFuncionario,
            Nombre:        funcionario.Nombre,
            Saldo:         funcionario.Saldo,
            Correo:        funcionario.Correo,
            Telefono:      funcionario.Telefono,
            Documento:     usuario?.Documento.ToString() ?? "—",
            Sede:          sede?.Nombre ?? "Sin sede"
        );
    }
}
