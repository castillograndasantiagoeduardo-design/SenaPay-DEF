using SenaPay.Application.DTOs.Categoria;
using SenaPay.Domain.Interfaces;

namespace SenaPay.Application.UseCases.AdminTienda;

public class GestionarCategoriasTiendaUseCase
{
    private readonly ICategoriaRepository _catRepo;
    private readonly ITiendaCategoriaRepository _tiendaCatRepo;

    public GestionarCategoriasTiendaUseCase(
        ICategoriaRepository catRepo,
        ITiendaCategoriaRepository tiendaCatRepo)
    {
        _catRepo = catRepo;
        _tiendaCatRepo = tiendaCatRepo;
    }

    public async Task<IEnumerable<CategoriaSeleccionDto>> ObtenerConEstadoAsync(int idTienda)
    {
        var todas = await _catRepo.ObtenerTodasAsync();
        var habilitadas = (await _tiendaCatRepo.ObtenerIdsHabilitadosPorTiendaAsync(idTienda)).ToHashSet();

        return todas.Select(c => new CategoriaSeleccionDto
        {
            IdCategoria = c.Id_Categoria,
            NombreCategoria = c.Nombre_Categoria!,
            Habilitada = habilitadas.Contains(c.Id_Categoria)
        });
    }

    public async Task GuardarSeleccionAsync(int idTienda, IEnumerable<int> ids)
        => await _tiendaCatRepo.HabilitarCategoriasAsync(idTienda, ids);

    public async Task ToggleAsync(int idTienda, int idCategoria, bool habilitar)
    {
        if (habilitar)
            await _tiendaCatRepo.HabilitarCategoriasAsync(idTienda, new[] { idCategoria });
        else
            await _tiendaCatRepo.DeshabilitarCategoriaAsync(idTienda, idCategoria);
    }
}