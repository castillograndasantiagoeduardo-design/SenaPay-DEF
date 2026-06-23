// SenaPay.Application/UseCases/Categorias/EditarCategoriaUseCase.cs
using SenaPay.Application.DTOs.Categorias;
using SenaPay.Domain.Interfaces;

namespace SenaPay.Application.UseCases.Categorias;

public class EditarCategoriaUseCase
{
    private readonly ICategoriaRepository _repo;

    public EditarCategoriaUseCase(ICategoriaRepository repo)
        => _repo = repo;

    public async Task<(bool Exito, string Mensaje, CategoriaDto? Categoria)> EjecutarAsync(
        int id, CrearCategoriaRequest request)
    {
        var categoria = await _repo.ObtenerPorIdAsync(id);

        if (categoria is null)
            return (false, "La categoría no existe.", null);

        // Verifica duplicado (excluyendo la misma categoría)
        var duplicado = await _repo.ExisteConNombreExcluyendoIdAsync(request.Nombre_Categoria, id);
        if (duplicado)
            return (false, $"Ya existe otra categoría con el nombre '{request.Nombre_Categoria}'.", null);

        categoria.ActualizarNombre(request.Nombre_Categoria);
        await _repo.GuardarCambiosAsync();

        return (true, "Categoría actualizada.", new CategoriaDto(categoria.Id_Categoria, categoria.Nombre_Categoria));
    }
}