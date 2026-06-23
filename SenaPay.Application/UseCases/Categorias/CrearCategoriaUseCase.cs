// SenaPay.Application/UseCases/Categorias/CrearCategoriaUseCase.cs
using SenaPay.Application.DTOs.Categorias;
using SenaPay.Domain.Entities;
using SenaPay.Domain.Interfaces;

namespace SenaPay.Application.UseCases.Categorias;

public class CrearCategoriaUseCase
{
    private readonly ICategoriaRepository _repo;

    public CrearCategoriaUseCase(ICategoriaRepository repo)
        => _repo = repo;

    /// <summary>
    /// Crea una nueva categoría. Retorna (true, categoría) o (false, mensaje de error).
    /// </summary>
    public async Task<(bool Exito, string Mensaje, CategoriaDto? Categoria)> EjecutarAsync(
        CrearCategoriaRequest request)
    {
        // Regla de negocio: no duplicar nombres (case-insensitive)
        if (await _repo.ExisteConNombreAsync(request.Nombre_Categoria))
            return (false, $"Ya existe una categoría llamada '{request.Nombre_Categoria}'.", null);

        var categoria = Categoria.Crear(request.Nombre_Categoria);
        await _repo.AgregarAsync(categoria);
        await _repo.GuardarCambiosAsync();

        var dto = new CategoriaDto(categoria.Id_Categoria, categoria.Nombre_Categoria);
        return (true, "Categoría creada exitosamente.", dto);
    }
}