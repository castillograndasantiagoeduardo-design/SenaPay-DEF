using SenaPay.Application.UseCases.Funcionarios.DTOs;
using SenaPay.Domain.Interfaces.Usuarios;

namespace SenaPay.Application.UseCases.Funcionarios;

public class ObtenerUsuarioUseCase
{
    private readonly IUsuarioRepository _repo;
    public ObtenerUsuarioUseCase(IUsuarioRepository repo) => _repo = repo;

    public async Task<UsuarioDto?> EjecutarAsync(int idUsuario)
    {
        var aprendiz = await _repo.ObtenerAprendizPorIdAsync(idUsuario);
        if (aprendiz is not null)
            return new UsuarioDto(idUsuario, aprendiz.Nombre, aprendiz.Correo,
                                  aprendiz.Telefono, string.Empty, aprendiz.Saldo,
                                  "Aprendiz", 1);

        var funcionario = await _repo.ObtenerFuncionarioPorIdAsync(idUsuario);
        if (funcionario is not null)
            return new UsuarioDto(idUsuario, funcionario.Nombre, funcionario.Correo,
                                  funcionario.Telefono, string.Empty, funcionario.Saldo,
                                  "Funcionario", 2);

        var admincafeteria = await _repo.ObtenerAdminCafeteriaPorIdAsync(idUsuario);
        if (admincafeteria is not null)
            return new UsuarioDto(idUsuario, admincafeteria.Nombre, admincafeteria.Correo,
                                  admincafeteria.Telefono, string.Empty, admincafeteria.Saldo,
                                  "AdminTienda", 3);

        return null;
    }
}