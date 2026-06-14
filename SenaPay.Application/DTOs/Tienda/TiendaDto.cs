namespace SenaPay.Application.DTOs.Tienda;

public record TiendaDto(
    int IdTienda,
    string NombreTienda,
    string Ubicacion,
    string NombreSede,
    string NombreAdmin
);

public record SedeDto(int IdSede, string Nombre, string Ciudad);