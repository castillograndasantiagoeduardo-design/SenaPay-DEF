// SenaPay.Application/DTOs/Categorias/CrearCategoriaRequest.cs
using System.ComponentModel.DataAnnotations;

namespace SenaPay.Application.DTOs.Categorias;

/// <summary>DTO de entrada para crear una categoría.</summary>
public class CrearCategoriaRequest
{
    [Required(ErrorMessage = "El nombre es obligatorio.")]
    [StringLength(80, MinimumLength = 2,
        ErrorMessage = "El nombre debe tener entre 2 y 80 caracteres.")]
    public string Nombre_Categoria { get; set; } = string.Empty;
}