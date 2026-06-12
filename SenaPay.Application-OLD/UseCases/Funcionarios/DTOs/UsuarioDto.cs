//Modelo de datos que se usaran en la logica Application 

namespace SenaPay.Application.UseCases.Funcionarios.DTOs;

/// <summary>
/// DTO de respuesta para el listado y detalle de usuarios.
/// Es lo único que la capa de Presentación conoce; nunca viajan entidades.
/// </summary>
public record UsuarioDto(
    int IdUsuario,
    string Nombre,
    string Correo,
    string Telefono,
    string Documento,
    decimal Saldo,
    string Rol,
    int IdRol
);

/// <summary>
/// DTO de entrada para agregar un usuario. Agrupa todos los parámetros
/// que antes llegaban sueltos al controlador.
/// </summary>
public record AgregarUsuarioRequest(
    string Nombre,
    string Correo,
    string Telefono,
    int Documento,
    int IdRol,
    decimal Saldo
);

/// <summary>
/// DTO de entrada para editar un usuario.
/// </summary>
public record EditarUsuarioRequest(
    int IdUsuario,
    string Nombre,
    string Correo,
    string Telefono,
    decimal Saldo
);

/// <summary>
/// Respuesta genérica para operaciones que solo indican éxito o error.
/// </summary>
public record OperacionResultado(bool Ok, string Mensaje);