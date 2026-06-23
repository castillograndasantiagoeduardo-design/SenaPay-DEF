using SenaPay.Domain.Entities;

namespace SenaPay.Domain.Interfaces;

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
    // ── NUEVO ──
    Task<List<AdminCafeteriaListItem>> ObtenerAdminCafeteriasAsync();
    Task<Aprendix?> ObtenerAprendizPorIdAsync(int idUsuario);
    Task<Funcionario?> ObtenerFuncionarioPorIdAsync(int idUsuario);
    Task<AdminCafeterium?> ObtenerAdminCafeteriaPorIdAsync(int idUsuario);
    Task EliminarConCascadaAsync(int idUsuario);
    Task CambiarEstadoAsync(int idUsuario, bool activo);
    Task<Usuario?> ObtenerPorIdSinFiltroAsync(int idUsuario);
    Task GuardarCambiosAsync();
    // Agrega estas líneas a la interfaz existente
    Task AgregarAdminCafeteriaAsync(AdminCafeterium adminCafeteria);
    // IUsuarioRepository + UsuarioRepository
    Task<AdminCafeterium?> ObtenerAdminCafeteriaPorPkAsync(int idAdminCafeteria);
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
    int IdRol,
    bool Activo      // ← nuevo campo
);

// ── NUEVO record ──
public record AdminCafeteriaListItem(
    int IdAdminCafeteria,
    string Nombre,
    string Correo,
    string Telefono,
    int TiendasAsignadas
);