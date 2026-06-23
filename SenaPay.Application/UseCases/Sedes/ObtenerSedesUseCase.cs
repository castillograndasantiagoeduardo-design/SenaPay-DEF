using SenaPay.Application.DTOs.Tienda;
using SenaPay.Domain.Interfaces;

namespace SenaPay.Application.UseCases.Sedes;

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