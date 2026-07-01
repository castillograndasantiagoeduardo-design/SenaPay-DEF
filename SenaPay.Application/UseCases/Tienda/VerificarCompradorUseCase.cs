using SenaPay.Domain.Interfaces;

namespace SenaPay.Application.UseCases.Tienda;

public class VerificarCompradorUseCase
{
    private readonly IAprendizRepository _aprendizRepo;

    public VerificarCompradorUseCase(IAprendizRepository aprendizRepo)
    {
        _aprendizRepo = aprendizRepo;
    }

    public async Task<(bool Encontrado, string Nombre, decimal Saldo)> EjecutarAsync(int documento)
    {
        var aprendiz = await _aprendizRepo.ObtenerPorDocumentoAsync(documento);
        if (aprendiz == null)
            return (false, string.Empty, 0);
        return (true, aprendiz.Nombre, aprendiz.Saldo);
    }
}
