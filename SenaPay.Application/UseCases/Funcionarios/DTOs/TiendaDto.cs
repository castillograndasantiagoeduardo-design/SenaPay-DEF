namespace SenaPay.Application.UseCases.Funcionarios.DTOs;

public record TiendaDto(
    int IdTienda,
    string NombreTienda,
    string Ubicacion,
    string NombreSede,
    string NombreAdmin
);

public record SedeDto(int IdSede, string Nombre, string Ciudad);