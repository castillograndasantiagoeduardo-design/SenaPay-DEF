using SenaPay.Domain.Interfaces.Usuarios;

namespace SenaPay.Application.UseCases.Funcionarios;

public sealed class CambiarEstadoReporteUseCase
{
    private readonly IReporteRepository _repo;
    public CambiarEstadoReporteUseCase(IReporteRepository repo) => _repo = repo;

    private static readonly HashSet<string> _estadosValidos =
        ["Pendiente", "En revisión", "Resuelto"];

    public async Task<bool> EjecutarAsync(int idReporte, string nuevoEstado)
    {
        if (!_estadosValidos.Contains(nuevoEstado)) return false;
        return await _repo.CambiarEstadoAsync(idReporte, nuevoEstado);
    }
}