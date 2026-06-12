using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SenaPay.Domain.Entities;
namespace SenaPay.Domain.Interfaces.Usuarios
{
    /// <summary>
    /// Contrato que define las operaciones de consulta sobre Aprendices.
    /// Vive en Dominio: no sabe nada de EF Core ni SQL Server.
    /// </summary>
    public interface IAprendizRepository
    {
        /// <summary>
        /// Busca un aprendiz por el documento del usuario dueño de ese perfil.
        /// Retorna null si no existe.
        /// </summary>
        Task<Aprendix?> ObtenerPorDocumentoAsync(int documento);
        Task<Aprendix?> ObtenerPorIdUsuarioAsync(int idUsuario);
        Task<bool> DescontarSaldoAsync(int idAprendiz, decimal monto);
        Task<decimal> ConsultarSaldoAsync(int idAprendiz);
    }
}
