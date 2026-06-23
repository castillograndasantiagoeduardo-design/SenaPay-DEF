using Microsoft.EntityFrameworkCore;
using SenaPay.Domain.Entities;
using SenaPay.Domain.Interfaces;
using SenaPay.Infrastructure.Data;

namespace SenaPay.Infrastructure.Repositories.Usuarios;

public class AccountRepository : IAccountRepository
{
    private readonly SenaPayContext _context;
    public AccountRepository(SenaPayContext context) => _context = context;

    public async Task<Usuario?> ObtenerUsuarioPorDocumentoYClaveAsync(int documento, string clave)
    {
        return await _context.Usuarios
            .Include(u => u.AdminCafeteria)
                .ThenInclude(a => a.Tienda)   // ← carga la tienda del admin
            .Include(u => u.Funcionarios)
            .Include(u => u.Aprendices)
            .FirstOrDefaultAsync(u =>
                u.Documento == documento && u.Clave == clave &&
                u.Activo);
    }

    public async Task<(Usuario? usuario, string? correo)> ObtenerUsuarioConCorreoAsync(int documento)
    {
        var usuario = await _context.Usuarios
            .Include(u => u.Aprendices)
            .Include(u => u.Funcionarios)
            .Include(u => u.AdminCafeteria)
            .FirstOrDefaultAsync(u => u.Documento == documento &&
                u.Activo);

        if (usuario is null) return (null, null);

        string? correo = usuario.Aprendices.FirstOrDefault()?.Correo
                      ?? usuario.Funcionarios.FirstOrDefault()?.Correo
                      ?? usuario.AdminCafeteria.FirstOrDefault()?.Correo;

        return (usuario, correo);
    }

    public async Task<RecuperacionPassword?> ObtenerRecuperacionActivaAsync(string token) =>
        await _context.RecuperacionPasswords
            .Include(r => r.IdUsuarioNavigation)
            .FirstOrDefaultAsync(r => r.Token == token
                                   && r.Usado == false
                                   && r.FechaExpiracion > DateTime.Now);

    public async Task AgregarRecuperacionAsync(RecuperacionPassword recuperacion) =>
        await _context.RecuperacionPasswords.AddAsync(recuperacion);

    public async Task GuardarCambiosAsync() =>
        await _context.SaveChangesAsync();
}