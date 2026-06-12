using SenaPay.Domain.Entities;
using SenaPay.Domain.Interfaces.Usuarios;
using SenaPay.Domain.ValueObjects;

namespace SenaPay.Application.UseCases.Funcionarios;

public sealed class ObtenerEstadisticasReportesUseCase
{
    private readonly IReporteRepository _repo;
    public ObtenerEstadisticasReportesUseCase(IReporteRepository repo) => _repo = repo;

    public Task<ReporteEstadisticas> EjecutarAsync()
        => _repo.ObtenerEstadisticasAsync();
}