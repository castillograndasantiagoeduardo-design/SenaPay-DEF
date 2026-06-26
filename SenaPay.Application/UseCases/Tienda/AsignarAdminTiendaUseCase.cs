// SenaPay.Application/UseCases/Tienda/AsignarAdminTiendaUseCase.cs
using SenaPay.Application.DTOs;
using SenaPay.Application.DTOs.Tienda;
using SenaPay.Domain.Interfaces;

namespace SenaPay.Application.UseCases.Tienda;

public class AsignarAdminTiendaUseCase
{
    private readonly ITiendaRepository _tiendaRepo;
    private readonly IUsuarioRepository _usuarioRepo;

    public AsignarAdminTiendaUseCase(
        ITiendaRepository tiendaRepo,
        IUsuarioRepository usuarioRepo)
    {
        _tiendaRepo = tiendaRepo;
        _usuarioRepo = usuarioRepo;
    }

    public async Task<ResultadoOperacion> EjecutarAsync(AsignarAdminRequest request)
    {
        // 1. Validar que la tienda existe
        var tienda = await _tiendaRepo.ObtenerPorIdAsync(request.IdTienda);
        if (tienda is null)
            return ResultadoOperacion.Error("La tienda no existe.");

        // 2. Si se envía un admin, validar que existe
        if (request.IdAdminCafeteria.HasValue)
        {
            var admin = await _usuarioRepo.ObtenerAdminCafeteriaPorPkAsync(
                            request.IdAdminCafeteria.Value);
            if (admin is null)
                return ResultadoOperacion.Error("El administrador seleccionado no existe.");
        }

        // 3. Ejecutar asignación (o desasignación si null)
        var ok = await _tiendaRepo.AsignarAdminAsync(
                     request.IdTienda,
                     request.IdAdminCafeteria);

        return ok
            ? ResultadoOperacion.Exito(
                request.IdAdminCafeteria.HasValue
                    ? "Administrador asignado correctamente."
                    : "Administrador desasignado correctamente.")
            : ResultadoOperacion.Error("No se pudo actualizar la tienda.");
    }
}