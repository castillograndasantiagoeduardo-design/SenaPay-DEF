// SenaPay.Application/UseCases/Tienda/SeleccionarTiendaUseCase.cs
using SenaPay.Domain.Interfaces;

namespace SenaPay.Application.UseCases.Tienda;

public record SeleccionarTiendaResultado(bool Ok, string Mensaje);

public class SeleccionarTiendaUseCase
{
    private readonly ITiendaRepository _tiendaRepo;

    public SeleccionarTiendaUseCase(ITiendaRepository tiendaRepo)
        => _tiendaRepo = tiendaRepo;

    /// <summary>
    /// Valida que la tienda seleccionada realmente pertenece al admin
    /// antes de guardarla en Session — evita manipulación de IDs.
    /// </summary>
    public async Task<SeleccionarTiendaResultado> EjecutarAsync(
        int idTienda, int idUsuario)
    {
        var tiendas = await _tiendaRepo.ObtenerPorAdminUsuarioAsync(idUsuario);

        bool esSuya = tiendas.Any(t => t.IdTienda == idTienda);

        if (!esSuya)
            return new SeleccionarTiendaResultado(false,
                "No tienes permisos sobre esa tienda.");

        return new SeleccionarTiendaResultado(true, "OK");
    }
}