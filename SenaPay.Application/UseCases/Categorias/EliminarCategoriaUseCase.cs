// SenaPay.Application/UseCases/Categorias/EliminarCategoriaUseCase.cs
using SenaPay.Domain.Interfaces;

namespace SenaPay.Application.UseCases.Categorias;

public class EliminarCategoriaUseCase
{
    private readonly ICategoriaRepository _repo;

    public EliminarCategoriaUseCase(ICategoriaRepository repo)
        => _repo = repo;

    public async Task<(bool Exito, string Mensaje)> EjecutarAsync(int id)
    {
        var categoria = await _repo.ObtenerPorIdAsync(id);

        if (categoria is null)
            return (false, "La categoría no existe.");

        await _repo.EliminarAsync(categoria);
        await _repo.GuardarCambiosAsync();

        return (true, $"Categoría '{categoria.Nombre_Categoria}' eliminada.");
    }
}