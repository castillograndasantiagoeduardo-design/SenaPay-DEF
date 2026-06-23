using SenaPay.Application.DTOs.Usuarios;
using SenaPay.Domain.Entities;
using SenaPay.Domain.Interfaces;

namespace SenaPay.Application.UseCases.Usuarios;

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
                                  "Aprendiz", 1, aprendiz.IdUsuarioNavigation?.Activo ?? true);

        var funcionario = await _repo.ObtenerFuncionarioPorIdAsync(idUsuario);
        if (funcionario is not null)
            return new UsuarioDto(idUsuario, funcionario.Nombre, funcionario.Correo,
                                  funcionario.Telefono, string.Empty, funcionario.Saldo,
                                  "Funcionario", 2, funcionario.IdUsuarioNavigation?.Activo ?? true);

        var admincafeteria = await _repo.ObtenerAdminCafeteriaPorIdAsync(idUsuario);
        if (admincafeteria is not null)
            return new UsuarioDto(idUsuario, admincafeteria.Nombre, admincafeteria.Correo,
                                  admincafeteria.Telefono, string.Empty, admincafeteria.Saldo,
                                  "AdminTienda", 3,admincafeteria.IdUsuarioNavigation?.Activo ?? true);

        return null;
    }
}