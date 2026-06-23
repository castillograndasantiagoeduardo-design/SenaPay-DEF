// SenaPay.Application/UseCases/Tienda/ObtenerTiendasDeAdminUseCase.cs
using SenaPay.Application.DTOs.Tienda;
using SenaPay.Domain.Interfaces;

namespace SenaPay.Application.UseCases.Tienda;

public class ObtenerTiendasDeAdminUseCase
{
    private readonly ITiendaRepository _tiendaRepo;

    public ObtenerTiendasDeAdminUseCase(ITiendaRepository tiendaRepo)
        => _tiendaRepo = tiendaRepo;

    public async Task<List<TiendaSeleccionDto>> EjecutarAsync(int idUsuario)
    {
        var tiendas = await _tiendaRepo.ObtenerPorAdminUsuarioAsync(idUsuario);

        return tiendas.Select(t => new TiendaSeleccionDto(
            t.IdTienda,
            t.Nombre,
            t.Ubicacion,
            t.IdSedeNavigation?.Nombre ?? "—"
        )).ToList();
    }
}