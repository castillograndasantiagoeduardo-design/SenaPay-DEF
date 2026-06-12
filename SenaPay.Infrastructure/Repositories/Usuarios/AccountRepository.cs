using Microsoft.EntityFrameworkCore;
using SenaPay.Domain.Entities;
using SenaPay.Domain.Interfaces.Usuarios;
using SenaPay.Infrastructure.Data;

namespace SenaPay.Infrastructure.Repositories.Usuarios;

public class AccountRepository : IAccountRepository
{
    private readonly SenaPayContext _context;
    public AccountRepository(SenaPayContext context) => _context = context;

    public async Task<Usuario?> ObtenerUsuarioPorDocumentoYClaveAsync(int documento, string clave) =>
        await _context.Usuarios
            .FirstOrDefaultAsync(u => u.Documento == documento && u.Clave == clave);

    public async Task<(Usuario? usuario, string? correo)> ObtenerUsuarioConCorreoAsync(int documento)
    {
        var usuario = await _context.Usuarios
            .Include(u => u.Aprendices)
            .Include(u => u.Funcionarios)
            .FirstOrDefaultAsync(u => u.Documento == documento);

        if (usuario is null) return (null, null);

        string? correo = usuario.Aprendices.FirstOrDefault()?.Correo
                      ?? _context.Funcionarios
                             .FirstOrDefault(f => f.IdUsuario == usuario.IdUsuario)?.Correo;

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