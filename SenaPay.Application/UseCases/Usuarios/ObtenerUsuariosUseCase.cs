using SenaPay.Application.DTOs.Usuarios;
using SenaPay.Domain.Interfaces;

namespace SenaPay.Application.UseCases.Usuarios;

public class ObtenerUsuariosUseCase
{
    private readonly IUsuarioRepository _repo;
    public ObtenerUsuariosUseCase(IUsuarioRepository repo) => _repo = repo;

    public async Task<List<UsuarioDto>> EjecutarAsync()
    {
        var lista = await _repo.ObtenerTodosAsync();

        return lista.Select(u => new UsuarioDto(
            u.IdUsuario, u.Nombre, u.Correo, u.Telefono,
            u.Documento, u.Saldo, u.Rol, u.IdRol, u.Activo
        )).ToList();
    }
}