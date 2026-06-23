// SenaPay.Application/DTOs/Categorias/CategoriaDto.cs
namespace SenaPay.Application.DTOs.Categorias;

/// <summary>DTO de salida — lo que se envía a la vista.</summary>
public record CategoriaDto(int Id_Categoria, string Nombre_Categoria);