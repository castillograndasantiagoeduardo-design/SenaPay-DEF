using Microsoft.EntityFrameworkCore;
using SenaPay.Domain.Entities;
using SenaPay.Domain.Interfaces.Usuarios;
using SenaPay.Domain.ValueObjects;
using SenaPay.Infrastructure.Data;
using System;

namespace SenaPay.Infrastructure.Repositories.Usuarios;

public class ReporteRepository : IReporteRepository
{
    private readonly SenaPayContext _context;

    public ReporteRepository(SenaPayContext context)
    {
        _context = context;
    }
    //El caso de uso llama a este metodo de IReporteRepository 
    //Agrega el reporte a la BSD y retorna un true o false dependiendo si se tuvo exito 
    public async Task<bool> CrearReporteAsync(Reporte reporte)
    {
        await _context.Reportes.AddAsync(reporte);
        int filasAfectadas = await _context.SaveChangesAsync();
        return filasAfectadas > 0;
    }

    public async Task<int?> ObtenerIdUsuarioPorDocumentoAsync(int documento)
    {
        // Busca el Id_Usuario cuyo Documento coincida (tabla Usuarios)
        var usuario = await _context.Usuarios
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Documento == documento);

        return usuario?.IdUsuario;
    }
    // ── Nuevos ────────────────────────────────────────────────
    public async Task<IEnumerable<Reporte>> ObtenerTodosAsync()
        => await _context.Reportes
            .AsNoTracking()
            .Include(r => r.Usuario)
            .OrderByDescending(r => r.Fecha_Creacion)
            .ToListAsync();

    public async Task<Reporte?> ObtenerPorIdAsync(int idReporte)
        => await _context.Reportes
            .AsNoTracking()
            .Include(r => r.Usuario)
            .FirstOrDefaultAsync(r => r.Id_Reporte == idReporte);

    public async Task<bool> CambiarEstadoAsync(int idReporte, string nuevoEstado)
    {
        var reporte = await _context.Reportes.FindAsync(idReporte);
        if (reporte is null) return false;

        reporte.Estado = nuevoEstado;
        if (nuevoEstado == "Resuelto")
            reporte.Fecha_Resolucion = DateTime.UtcNow;

        return await _context.SaveChangesAsync() > 0;
    }

   public async Task<ReporteEstadisticas> ObtenerEstadisticasAsync()
{
    var stats = await _context.Reportes
        .AsNoTracking()
        .GroupBy(_ => 1)
        .Select(g => new
        {
            Total      = g.Count(),
            Pendientes = g.Count(r => r.Estado == "Pendiente"),
            EnRevision = g.Count(r => r.Estado == "En revisión"),
            Resueltos  = g.Count(r => r.Estado == "Resuelto")
        })
        .FirstOrDefaultAsync();

    // Si no hay reportes, devuelve el Value Object vacío en lugar de null
    return stats is null
        ? ReporteEstadisticas.Vacio()
        : ReporteEstadisticas.Crear(
            stats.Total,
            stats.Pendientes,
            stats.EnRevision,
            stats.Resueltos);
}
}