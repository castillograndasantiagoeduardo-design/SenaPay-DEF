// SenaPay.Application/UseCases/Categorias/ObtenerCategoriasUseCase.cs
using SenaPay.Application.DTOs.Categorias;
using SenaPay.Domain.Interfaces;

namespace SenaPay.Application.UseCases.Categorias;

public class ObtenerCategoriasUseCase
{
    private readonly ICategoriaRepository _repo;

    public ObtenerCategoriasUseCase(ICategoriaRepository repo)
        => _repo = repo;

    public async Task<IEnumerable<CategoriaDto>> EjecutarAsync()
    {
        var categorias = await _repo.ObtenerTodasAsync();

        return categorias
            .OrderBy(c => c.Nombre_Categoria)
            .Select(c => new CategoriaDto(c.Id_Categoria, c.Nombre_Categoria));
    }
}