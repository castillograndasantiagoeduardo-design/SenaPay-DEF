namespace SenaPay.Application.DTOs.Tienda;
using SenaPay.Domain.Enums;

/// <summary>
/// Datos que llegan desde el formulario del Funcionario
/// para crear una Tienda y su Administrador en un solo paso.
/// </summary>
public record CrearTiendaRequest(
    // Datos de la tienda
    string NombreTienda,
    string Ubicacion,
    int IdSede,

    // Modo de asignación — enum tipado, sin magic strings
    ModoAsignacionAdmin ModoAdmin,

    // Solo requerido si ModoAdmin == Existente
    int? IdAdminExistente,

    // Datos del usuario AdminTienda (Rol 3) a crear
    string? NombreAdmin,
    string? CorreoAdmin,
    string? TelefonoAdmin,
    int? DocumentoAdmin
);
public record CrearTiendaResultado(bool Ok, string Mensaje, int? IdTienda = null);