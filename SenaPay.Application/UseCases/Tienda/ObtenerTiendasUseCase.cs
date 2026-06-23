// SenaPay.Application/UseCases/Tienda/ObtenerTiendasUseCase.cs
using SenaPay.Application.DTOs.Tienda;
using SenaPay.Domain.Interfaces;

namespace SenaPay.Application.UseCases.Tienda;

public class ObtenerTiendasUseCase
{
    private readonly ITiendaRepository _tiendaRepo;
    public ObtenerTiendasUseCase(ITiendaRepository tiendaRepo) => _tiendaRepo = tiendaRepo;

    public async Task<List<TiendaDto>> EjecutarAsync()
    {
        var tiendas = await _tiendaRepo.ObtenerTodasAsync();

        return tiendas.Select(t => new TiendaDto(
            t.IdTienda,
            t.Nombre,
            t.IdSedeNavigation?.Nombre ?? "—",
            t.Ubicacion,
            t.IdAdminCafeteriaNavigation?.Nombre ?? "—"
        )).ToList();
    }
}