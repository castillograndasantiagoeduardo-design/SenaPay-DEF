using SenaPay.Application.UseCases.Funcionarios.DTOs;
using SenaPay.Domain.Interfaces.Core;
using SenaPay.Domain.Interfaces.Tienda;

namespace SenaPay.Application.UseCases.Funcionarios;

public class ObtenerSedesUseCase
{
    private readonly ISedeRepository _sedeRepo;

    public ObtenerSedesUseCase(ISedeRepository sedeRepo)
        => _sedeRepo = sedeRepo;

    public async Task<List<SedeDto>> EjecutarAsync()
    {
        var sedes = await _sedeRepo.ObtenerTodasAsync();
        return sedes.Select(s => new SedeDto(s.IdSede, s.Nombre ?? "", s.Ciudad ?? ""))
                    .ToList();
    }
}