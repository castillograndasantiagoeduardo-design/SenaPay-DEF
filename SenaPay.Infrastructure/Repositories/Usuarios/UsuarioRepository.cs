using Microsoft.EntityFrameworkCore;
using SenaPay.Domain.Entities;
using SenaPay.Domain.Interfaces;
using SenaPay.Infrastructure.Data;

namespace SenaPay.Infrastructure.Repositories.Usuarios;

public class UsuarioRepository : IUsuarioRepository
{
    private readonly SenaPayContext _context;
    public UsuarioRepository(SenaPayContext context) => _context = context;

    public async Task<bool> ExisteDocumentoAsync(int documento) =>
        await _context.Usuarios.AnyAsync(u => u.Documento == documento);

    public async Task<Usuario?> ObtenerPorIdAsync(int idUsuario) =>
        await _context.Usuarios
            .Where(u => u.Activo)
            .FirstOrDefaultAsync(u => u.IdUsuario == idUsuario);

    public async Task AgregarAsync(Usuario usuario) =>
        await _context.Usuarios.AddAsync(usuario);

    public async Task AgregarAprendizAsync(Aprendix aprendiz) =>
        await _context.Aprendices.AddAsync(aprendiz);

    public async Task AgregarFuncionarioAsync(Funcionario funcionario) =>
        await _context.Funcionarios.AddAsync(funcionario);

    public async Task<List<UsuarioListItem>> ObtenerTodosAsync()
    {
        // ── Traer a memoria primero, LUEGO proyectar ──────────────
        var aprendices = await _context.Aprendices
            .Include(a => a.IdUsuarioNavigation)
            .ToListAsync();  // ← primero a memoria

        var funcionarios = await _context.Funcionarios
            .Include(f => f.IdUsuarioNavigation)
            .ToListAsync();

        var admincafeterias = await _context.AdminCafeteria
            .Include(a => a.IdUsuarioNavigation)
            .ToListAsync();

        // ── Proyectar en memoria — aquí Activo SÍ se lee bien ────
        var lista = aprendices.Select(a => new UsuarioListItem(
                a.IdUsuario,
                a.Nombre, a.Correo, a.Telefono,
                a.IdUsuarioNavigation.Documento.ToString(),
                a.Saldo, "Aprendiz", 1,
                a.IdUsuarioNavigation.Activo   // ← ahora sí funciona
            ))
            .Concat(funcionarios.Select(f => new UsuarioListItem(
                f.IdUsuario,
                f.Nombre, f.Correo, f.Telefono,
                f.IdUsuarioNavigation.Documento.ToString(),
                f.Saldo, "Funcionario", 2,
                f.IdUsuarioNavigation.Activo
            )))
            .Concat(admincafeterias.Select(a => new UsuarioListItem(
                a.IdUsuario,
                a.Nombre, a.Correo, a.Telefono,
                a.IdUsuarioNavigation.Documento.ToString(),
                a.Saldo, "AdminTienda", 3,
                a.IdUsuarioNavigation.Activo
            )))
            .ToList();

        return lista;
    }

    public async Task<Aprendix?> ObtenerAprendizPorIdAsync(int idUsuario) =>
        await _context.Aprendices
            .Include(a => a.IdUsuarioNavigation)
            //.Where(a => a.IdUsuarioNavigation.Activo)
            .FirstOrDefaultAsync(a => a.IdUsuario == idUsuario);

    public async Task<Funcionario?> ObtenerFuncionarioPorIdAsync(int idUsuario) =>
        await _context.Funcionarios
            .Include(f => f.IdUsuarioNavigation)
            //.Where(f => f.IdUsuarioNavigation.Activo)
            .FirstOrDefaultAsync(f => f.IdUsuario == idUsuario);

    public async Task<AdminCafeterium?> ObtenerAdminCafeteriaPorIdAsync(int idUsuario) =>
        await _context.AdminCafeteria
            .Include(a => a.IdUsuarioNavigation)
            //.Where(a => a.IdUsuarioNavigation.Activo)
            .FirstOrDefaultAsync(a => a.IdUsuario == idUsuario);

    public async Task EliminarConCascadaAsync(int idUsuario)
    {
        // 1. Si es AdminCafeteria, desasociar sus tiendas
        var adminCafeteria = await _context.AdminCafeteria
            .FirstOrDefaultAsync(a => a.IdUsuario == idUsuario);

        if (adminCafeteria is not null)
        {
            var tiendas = await _context.Tienda
                .Where(t => t.IdAdminCafeteria == adminCafeteria.IdAdminCafeteria)
                .ToListAsync();

            foreach (var tienda in tiendas)
                tienda.IdAdminCafeteria = null;
        }

        // 2. Limpiar tokens de recuperación de contraseña
        var recuperaciones = _context.RecuperacionPasswords
            .Where(r => r.IdUsuario == idUsuario);
        _context.RecuperacionPasswords.RemoveRange(recuperaciones);

        // 3. Soft delete — única modificación al usuario
        var usuario = await _context.Usuarios.FindAsync(idUsuario);
        if (usuario is null) return;

        usuario.Activo = false;

        await _context.SaveChangesAsync();
    }

    public async Task CambiarEstadoAsync(int idUsuario, bool activo)
    {
        var usuario = await _context.Usuarios.FindAsync(idUsuario);
        if (usuario is null) return;

        usuario.Activo = activo;

        // Si se reactiva un AdminCafeteria, no hace falta nada extra
        // Si se desactiva, desasociar tiendas
        if (!activo)
        {
            var adminCafeteria = await _context.AdminCafeteria
                .FirstOrDefaultAsync(a => a.IdUsuario == idUsuario);

            if (adminCafeteria is not null)
            {
                var tiendas = await _context.Tienda
                    .Where(t => t.IdAdminCafeteria == adminCafeteria.IdAdminCafeteria)
                    .ToListAsync();

                foreach (var tienda in tiendas)
                    tienda.IdAdminCafeteria = null;
            }
        }

        await _context.SaveChangesAsync();
    }

    // Necesario para poder reactivar usuarios desactivados
    public async Task<Usuario?> ObtenerPorIdSinFiltroAsync(int idUsuario) =>
        await _context.Usuarios.FindAsync(idUsuario);

    public async Task AgregarAdminCafeteriaAsync(AdminCafeterium adminCafeteria) =>
        await _context.AdminCafeteria.AddAsync(adminCafeteria);

    public async Task GuardarCambiosAsync() =>
        await _context.SaveChangesAsync();

    public async Task<List<AdminCafeteriaListItem>> ObtenerAdminCafeteriasAsync()
    {
        var admins = await _context.AdminCafeteria
            .Include(a => a.IdUsuarioNavigation)
            .Include(a => a.Tienda)
            .Where(a => a.IdUsuarioNavigation.Activo)
            .OrderBy(a => a.Nombre)
            .ToListAsync();

        return admins.Select(a => new AdminCafeteriaListItem(
            a.IdAdminCafeteria,
            a.Nombre,
            a.Correo,
            a.Telefono,
            a.Tienda.Count
        )).ToList();
    }

    public async Task<AdminCafeterium?> ObtenerAdminCafeteriaPorPkAsync(int idAdminCafeteria) =>
        await _context.AdminCafeteria
            .Include(a => a.IdUsuarioNavigation)
            .Where(a => a.IdUsuarioNavigation.Activo)
            .FirstOrDefaultAsync(a => a.IdAdminCafeteria == idAdminCafeteria);
}