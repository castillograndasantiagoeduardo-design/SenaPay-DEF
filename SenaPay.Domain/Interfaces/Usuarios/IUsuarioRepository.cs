using SenaPay.Domain.Entities;

namespace SenaPay.Domain.Interfaces.Usuarios;

/// <summary>
/// Contrato de todas las operaciones CRUD sobre usuarios y sus perfiles.
/// Sin EF Core, sin SQL; solo la intención de negocio.
/// </summary>
public interface IUsuarioRepository
{
    Task<bool> ExisteDocumentoAsync(int documento);
    Task<Usuario?> ObtenerPorIdAsync(int idUsuario);
    Task AgregarAsync(Usuario usuario);
    Task AgregarAprendizAsync(Aprendix aprendiz);
    Task AgregarFuncionarioAsync(Funcionario funcionario);
    Task<List<UsuarioListItem>> ObtenerTodosAsync();
    Task<Aprendix?> ObtenerAprendizPorIdAsync(int idUsuario);
    Task<Funcionario?> ObtenerFuncionarioPorIdAsync(int idUsuario);
    Task<AdminCafeterium?> ObtenerAdminCafeteriaPorIdAsync(int idUsuario);
    Task EliminarConCascadaAsync(int idUsuario);
    Task GuardarCambiosAsync();
    // Agrega estas líneas a la interfaz existente
    Task AgregarAdminCafeteriaAsync(AdminCafeterium adminCafeteria);
}

/// <summary>
/// Proyección plana usada para listar usuarios (evita exponer entidades
/// de dominio completas innecesariamente en la capa de Aplicación).
/// </summary>
public record UsuarioListItem(
    int IdUsuario,
    string Nombre,
    string Correo,
    string Telefono,
    string Documento,
    decimal Saldo,
    string Rol,
    int IdRol
);