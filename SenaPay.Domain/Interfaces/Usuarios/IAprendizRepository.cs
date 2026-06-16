using SenaPay.Domain.Entities;

namespace SenaPay.Domain.Interfaces.Usuarios;

/// <summary>
/// Contrato que define las operaciones de consulta sobre Aprendices.
/// Vive en Dominio: no sabe nada de EF Core ni SQL Server.
/// </summary>
public interface IAprendizRepository
{
    /// <summary>Busca un aprendiz por el documento del usuario dueño de ese perfil.</summary>
    Task<Aprendix?> ObtenerPorDocumentoAsync(int documento);

    Task<Aprendix?> ObtenerPorIdUsuarioAsync(int idUsuario);

    /// <summary>
    /// Devuelve el aprendiz con sus últimas N transacciones y sus detalles
    /// (para el historial del dashboard). Incluye Sede del usuario.
    /// </summary>
    Task<Aprendix?> ObtenerPerfilCompletoAsync(int documento, int cantidadTransacciones = 10);

    Task<bool> DescontarSaldoAsync(int idAprendiz, decimal monto);
    Task<decimal> ConsultarSaldoAsync(int idAprendiz);
}