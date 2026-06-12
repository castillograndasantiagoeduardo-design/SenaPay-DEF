using Microsoft.EntityFrameworkCore;
using SenaPay.Domain.Entities;
using SenaPay.Domain.Interfaces.Usuarios;
using SenaPay.Infrastructure.Data;

namespace SenaPay.Infrastructure.Repositories.Usuarios;

public class UsuarioRepository : IUsuarioRepository
{
    private readonly SenaPayContext _context;
    public UsuarioRepository(SenaPayContext context) => _context = context;

    public async Task<bool> ExisteDocumentoAsync(int documento) =>
        await _context.Usuarios.AnyAsync(u => u.Documento == documento);

    public async Task<Usuario?> ObtenerPorIdAsync(int idUsuario) =>
        await _context.Usuarios.FindAsync(idUsuario);

    public async Task AgregarAsync(Usuario usuario) =>
        await _context.Usuarios.AddAsync(usuario);

    public async Task AgregarAprendizAsync(Aprendix aprendiz) =>
        await _context.Aprendices.AddAsync(aprendiz);

    public async Task AgregarFuncionarioAsync(Funcionario funcionario) =>
        await _context.Funcionarios.AddAsync(funcionario);

    public async Task<List<UsuarioListItem>> ObtenerTodosAsync()
    {
        var aprendices = await _context.Aprendices
            .Include(a => a.IdUsuarioNavigation)
            .Select(a => new UsuarioListItem(
                a.IdUsuario,
                a.Nombre, a.Correo, a.Telefono,
                a.IdUsuarioNavigation.Documento.ToString(),
                a.Saldo, "Aprendiz", 1
            )).ToListAsync();

        var funcionarios = await _context.Funcionarios
            .Include(f => f.IdUsuarioNavigation)
            .Select(f => new UsuarioListItem(
                f.IdUsuario,
                f.Nombre, f.Correo, f.Telefono,
                f.IdUsuarioNavigation.Documento.ToString(),
                f.Saldo, "Funcionario", 2
            )).ToListAsync();

        var admincafeterias = await _context.AdminCafeteria
            .Include(a => a.IdUsuarioNavigation)
            .Select(a => new UsuarioListItem(
                a.IdUsuario,
                a.Nombre, a.Correo, a.Telefono,
                a.IdUsuarioNavigation.Documento.ToString(),
                a.Saldo, "AdminTienda", 3
            )).ToListAsync();

        return aprendices.Concat(funcionarios).Concat(admincafeterias).ToList();
    }

    public async Task<Aprendix?> ObtenerAprendizPorIdAsync(int idUsuario) =>
        await _context.Aprendices.FirstOrDefaultAsync(a => a.IdUsuario == idUsuario);

    public async Task<Funcionario?> ObtenerFuncionarioPorIdAsync(int idUsuario) =>
        await _context.Funcionarios.FirstOrDefaultAsync(f => f.IdUsuario == idUsuario);
    public async Task<AdminCafeterium?> ObtenerAdminCafeteriaPorIdAsync(int idUsuario) =>
        await _context.AdminCafeteria.FirstOrDefaultAsync(f => f.IdUsuario == idUsuario);

    public async Task EliminarConCascadaAsync(int idUsuario)
    {
        // 1. Recuperaciones de contraseña
        var recuperaciones = _context.RecuperacionPasswords
            .Where(r => r.IdUsuario == idUsuario);
        _context.RecuperacionPasswords.RemoveRange(recuperaciones);

        // 2. Perfil Aprendiz
        var aprendiz = await _context.Aprendices
            .FirstOrDefaultAsync(a => a.IdUsuario == idUsuario);
        if (aprendiz is not null) _context.Aprendices.Remove(aprendiz);

        // 3. Perfil Funcionario
        var funcionario = await _context.Funcionarios
            .FirstOrDefaultAsync(f => f.IdUsuario == idUsuario);
        if (funcionario is not null) _context.Funcionarios.Remove(funcionario);

        // 4. Usuario base
        var usuario = await _context.Usuarios.FindAsync(idUsuario);
        if (usuario is not null) _context.Usuarios.Remove(usuario);

        await _context.SaveChangesAsync();
    }

    // Agrega este método a la implementación existente
    public async Task AgregarAdminCafeteriaAsync(AdminCafeterium adminCafeteria) =>
        await _context.AdminCafeteria.AddAsync(adminCafeteria);

    public async Task GuardarCambiosAsync() =>
        await _context.SaveChangesAsync();
}