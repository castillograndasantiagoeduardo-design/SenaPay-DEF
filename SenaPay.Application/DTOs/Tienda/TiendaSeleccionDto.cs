// SenaPay.Application/DTOs/Tienda/TiendaSeleccionDto.cs
namespace SenaPay.Application.DTOs.Tienda;

public record TiendaSeleccionDto(
    int IdTienda,
    string Nombre,
    string Ubicacion,
    string NombreSede
);