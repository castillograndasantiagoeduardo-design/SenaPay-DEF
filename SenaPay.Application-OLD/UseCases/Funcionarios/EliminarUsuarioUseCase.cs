using SenaPay.Application.UseCases.Funcionarios.DTOs;
using SenaPay.Domain.Interfaces.Usuarios;

namespace SenaPay.Application.UseCases.Funcionarios;

public class EliminarUsuarioUseCase
{
    private readonly IUsuarioRepository _repo;
    public EliminarUsuarioUseCase(IUsuarioRepository repo) => _repo = repo;

    public async Task<OperacionResultado> EjecutarAsync(int idUsuario)
    {
        var usuario = await _repo.ObtenerPorIdAsync(idUsuario);
        if (usuario is null)
            return new OperacionResultado(false, "Usuario no encontrado.");

        try
        {
            await _repo.EliminarConCascadaAsync(idUsuario);
            return new OperacionResultado(true, "Usuario eliminado correctamente.");
        }
        catch (Exception ex)
        {
            Console.WriteLine("ERROR al eliminar: " + ex.Message);
            return new OperacionResultado(false, "No se pudo eliminar. Puede tener registros relacionados.");
        }
    }
}