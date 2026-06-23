// SenaPay.Application/UseCases/Tienda/ObtenerAdminsDisponiblesUseCase.cs
using SenaPay.Domain.Interfaces;

namespace SenaPay.Application.UseCases.Tienda;

public class ObtenerAdminsDisponiblesUseCase
{
    private readonly IUsuarioRepository _usuarioRepo;
    public ObtenerAdminsDisponiblesUseCase(IUsuarioRepository usuarioRepo)
        => _usuarioRepo = usuarioRepo;

    public async Task<List<AdminCafeteriaListItem>> EjecutarAsync()
        => await _usuarioRepo.ObtenerAdminCafeteriasAsync();
}