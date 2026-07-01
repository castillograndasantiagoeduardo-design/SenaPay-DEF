using SenaPay.Domain.Entities;

namespace SenaPay.Domain.Interfaces;

public interface IFuncionarioRepository
{
    Task<Funcionario?> ObtenerPorDocumentoAsync(int documento);
}
