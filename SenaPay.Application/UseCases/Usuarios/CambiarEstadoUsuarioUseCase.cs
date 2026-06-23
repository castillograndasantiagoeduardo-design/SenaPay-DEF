using SenaPay.Application.DTOs.Usuarios;
using SenaPay.Domain.Interfaces;

public class CambiarEstadoUsuarioUseCase
{
    private readonly IUsuarioRepository _repo;
    public CambiarEstadoUsuarioUseCase(IUsuarioRepository repo) => _repo = repo;

    public async Task<OperacionResultado> EjecutarAsync(int idUsuario, bool activar)
    {
        var usuario = await _repo.ObtenerPorIdAsync(idUsuario);

        // ObtenerPorIdAsync filtra Activo=true, 
        // para reactivar necesitamos buscar sin ese filtro
        if (usuario is null && activar)
            usuario = await _repo.ObtenerPorIdSinFiltroAsync(idUsuario);

        if (usuario is null)
            return new OperacionResultado(false, "Usuario no encontrado.");

        await _repo.CambiarEstadoAsync(idUsuario, activar);

        var mensaje = activar ? "Usuario activado correctamente."
                               : "Usuario desactivado correctamente.";
        return new OperacionResultado(true, mensaje);
    }
}