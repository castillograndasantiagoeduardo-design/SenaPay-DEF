namespace SenaPay.Application.DTOs.Categoria;

public class CategoriaSeleccionDto
{
    public int IdCategoria { get; set; }
    public string NombreCategoria { get; set; } = null!;
    public bool Habilitada { get; set; }
}