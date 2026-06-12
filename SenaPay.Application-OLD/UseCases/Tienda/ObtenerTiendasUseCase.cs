using SenaPay.Application.UseCases.Funcionarios.DTOs;
using SenaPay.Domain.Interfaces.Core;
using SenaPay.Domain.Interfaces.Tienda;

namespace SenaPay.Application.UseCases.Funcionarios;

public class ObtenerTiendasUseCase
{
    private readonly ITiendaRepository _tiendaRepo;

    public ObtenerTiendasUseCase(ITiendaRepository tiendaRepo)
        => _tiendaRepo = tiendaRepo;

    public async Task<List<TiendaDto>> EjecutarAsync()
    {
        var tiendas = await _tiendaRepo.ObtenerTodasAsync();

        return tiendas.Select(t => new TiendaDto(
            t.IdTienda,
            t.Nombre ?? string.Empty,
            t.Ubicacion ?? string.Empty,
            t.IdSedeNavigation?.Nombre ?? "—",
            t.IdAdminCafeteriaNavigation?.Nombre ?? "—"
        )).ToList();
    }
}