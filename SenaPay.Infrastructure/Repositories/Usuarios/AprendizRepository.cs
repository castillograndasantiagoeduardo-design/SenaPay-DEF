using Microsoft.EntityFrameworkCore; //Importa los superpoderes de Entity Framework Core. Gracias a esta línea, el archivo puede usar métodos avanzados de bases de datos como el .Include() o el .FirstOrDefaultAsync().
using SenaPay.Domain.Entities;//Le da acceso a tus entidades de base de datos (Aprendix, Usuario, etc.) que guardamos en la capa de Dominio.
using SenaPay.Domain.Interfaces;
using SenaPay.Infrastructure.Data;

namespace SenaPay.Infrastructure.Repositories.Usuarios;

/// <summary>
/// Implementación concreta: aquí vive EF Core.
/// El resto de las capas nunca importan este archivo directamente.
/// </summary>
public class AprendizRepository : IAprendizRepository //Een esta clase el sistema se compromete a cumplir  con lo que promete el contrato de IAprendizRepository
{
    //Puente para hablar con la base de datos 
    private readonly SenaPayContext _context;

    //Constructor que almacena en _context el contexto de datos de la BSD para poder hacer las consultas
    public AprendizRepository(SenaPayContext context)
    {
        _context = context;
    }
    //Es el método exacto que te exigía el contrato del Dominio. 
    //Recibe el número de documento (int documento) y promete buscar un aprendiz de manera asíncrona (async Task)
    // devolviendo los datos o un vacío (null) si no lo encuentra.
    public async Task<Aprendix?> ObtenerPorDocumentoAsync(int documento)
    {
        //Le dice a Entity Framework Core: "Entra a SQL Server y prepárate para buscar en la tabla de Aprendices".
        //El await hace que el sistema espere a que la base de datos responda sin congelar la aplicación web.
        return await _context.Aprendices
            .Include(a => a.IdUsuarioNavigation)//Join entre las tablas aprendices y usuarios, se traen de paso los datos del Usuario
            .FirstOrDefaultAsync(a => a.IdUsuarioNavigation.Documento == documento);
    }
    public async Task<Aprendix?> ObtenerPorIdUsuarioAsync(int idUsuario)
    {
        return await _context.Aprendices
            .FirstOrDefaultAsync(a => a.IdAprendiz == idUsuario);
    }

    public async Task<bool> DescontarSaldoAsync(int idAprendiz, decimal monto)
    {
        var aprendiz = await _context.Aprendices.FindAsync(idAprendiz);
        if (aprendiz == null || aprendiz.Saldo < monto) return false;

        aprendiz.Saldo -= monto;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<decimal> ConsultarSaldoAsync(int idAprendiz)
    {
        var aprendiz = await _context.Aprendices.FindAsync(idAprendiz);
        return aprendiz?.Saldo ?? 0m;
    }

    /// <summary>
    /// Trae el aprendiz con Sede + las últimas N transacciones y su primer detalle
    /// (para mostrar nombre del producto en el historial).
    /// </summary>
    public async Task<Aprendix?> ObtenerPerfilCompletoAsync(int documento, int cantidadTransacciones = 10)
    {
        return await _context.Aprendices
            // ── Datos del Usuario (documento, rol, sede) ──────────────────
            .Include(a => a.IdUsuarioNavigation)
                .ThenInclude(u => u.IdSedeNavigation)
            // ── Últimas transacciones ordenadas por fecha ─────────────────
            .Include(a => a.Transacciones.OrderByDescending(t => t.Fecha)
                                         .Take(cantidadTransacciones))
                .ThenInclude(t => t.DetalleTransaccions)
                    .ThenInclude(d => d.IdProductoNavigation)
            .FirstOrDefaultAsync(a => a.IdUsuarioNavigation.Documento == documento);
    }

    public async Task<bool> ActualizarPerfilAsync(int documento, string correo, string? telefono)
    {
        var aprendiz = await _context.Aprendices
            .Include(a => a.IdUsuarioNavigation)
            .FirstOrDefaultAsync(a => a.IdUsuarioNavigation.Documento == documento);

        if (aprendiz is null) return false;

        aprendiz.Correo = correo;
        aprendiz.Telefono = telefono;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<decimal> RecargarSaldoAtomicoAsync(int idAprendiz, int idUsuario, decimal monto, string descripcion)
    {
        await using var tx = await _context.Database.BeginTransactionAsync();
        try
        {
            var aprendiz = await _context.Aprendices.FindAsync(idAprendiz)
                ?? throw new InvalidOperationException("Aprendiz no encontrado.");

            aprendiz.Saldo += monto;

            _context.MovimientoSaldos.Add(new SenaPay.Domain.Entities.MovimientoSaldo
            {
                IdUsuario = idUsuario,
                TipoMovimiento = "RECARGA",
                Monto = monto,
                SaldoResultante = aprendiz.Saldo,
                Fecha = DateTime.Now,
                Descripcion = descripcion
            });

            await _context.SaveChangesAsync();
            await tx.CommitAsync();
            return aprendiz.Saldo;
        }
        catch
        {
            await tx.RollbackAsync();
            throw;
        }
    }
}
