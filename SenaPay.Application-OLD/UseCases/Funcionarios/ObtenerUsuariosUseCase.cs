using SenaPay.Application.UseCases.Funcionarios.DTOs;
using SenaPay.Domain.Interfaces.Usuarios;

namespace SenaPay.Application.UseCases.Funcionarios;

public class ObtenerUsuariosUseCase
{
    private readonly IUsuarioRepository _repo;
    public ObtenerUsuariosUseCase(IUsuarioRepository repo) => _repo = repo;

    public async Task<List<UsuarioDto>> EjecutarAsync()
    {
        var lista = await _repo.ObtenerTodosAsync();

        return lista.Select(u => new UsuarioDto(
            u.IdUsuario, u.Nombre, u.Correo, u.Telefono,
            u.Documento, u.Saldo, u.Rol, u.IdRol
        )).ToList();
    }
}