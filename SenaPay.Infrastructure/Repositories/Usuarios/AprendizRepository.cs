using Microsoft.EntityFrameworkCore; //Importa los superpoderes de Entity Framework Core. Gracias a esta línea, el archivo puede usar métodos avanzados de bases de datos como el .Include() o el .FirstOrDefaultAsync().
using SenaPay.Domain.Entities;//Le da acceso a tus entidades de base de datos (Aprendix, Usuario, etc.) que guardamos en la capa de Dominio.
using SenaPay.Domain.Interfaces.Usuarios;
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
}