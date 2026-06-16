using Microsoft.EntityFrameworkCore;
using SenaPay.Domain.Entities;
using SenaPay.Domain.Interfaces.Usuarios;
using SenaPay.Infrastructure.Data;

namespace SenaPay.Infrastructure.Repositories.Usuarios;

/// <summary>
/// Implementación concreta: aquí vive EF Core.
/// El resto de las capas nunca importan este archivo directamente.
/// </summary>
public class AprendizRepository : IAprendizRepository
{
    private readonly SenaPayContext _context;

    public AprendizRepository(SenaPayContext context)
    {
        _context = context;
    }

    /// <inheritdoc/>
    public async Task<Aprendix?> ObtenerPorDocumentoAsync(int documento)
    {
        return await _context.Aprendices
            .Include(a => a.IdUsuarioNavigation)
            .FirstOrDefaultAsync(a => a.IdUsuarioNavigation.Documento == documento);
    }

    /// <inheritdoc/>
    public async Task<Aprendix?> ObtenerPorIdUsuarioAsync(int idUsuario)
    {
        return await _context.Aprendices
            .FirstOrDefaultAsync(a => a.IdAprendiz == idUsuario);
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

    /// <inheritdoc/>
    public async Task<bool> DescontarSaldoAsync(int idAprendiz, decimal monto)
    {
        var aprendiz = await _context.Aprendices.FindAsync(idAprendiz);
        if (aprendiz == null || aprendiz.Saldo < monto) return false;
        aprendiz.Saldo -= monto;
        await _context.SaveChangesAsync();
        return true;
    }

    /// <inheritdoc/>
    public async Task<decimal> ConsultarSaldoAsync(int idAprendiz)
    {
        var aprendiz = await _context.Aprendices.FindAsync(idAprendiz);
        return aprendiz?.Saldo ?? 0m;
    }
}