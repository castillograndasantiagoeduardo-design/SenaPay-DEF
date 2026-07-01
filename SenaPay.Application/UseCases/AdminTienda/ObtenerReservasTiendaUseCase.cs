using SenaPay.Application.DTOs.Venta;
using SenaPay.Domain.Interfaces;

namespace SenaPay.Application.UseCases.AdminTienda;

public class ObtenerReservasTiendaUseCase
{
    private readonly ITransaccionRepository _repo;

    public ObtenerReservasTiendaUseCase(ITransaccionRepository repo) => _repo = repo;

    public async Task<List<ReservaTiendaDto>> EjecutarAsync(int idTienda)
    {
        var reservas = await _repo.ObtenerReservasPorTiendaAsync(idTienda);
        return reservas.Select(t => new ReservaTiendaDto(
            t.IdTransaccion,
            t.IdAprendizNavigation?.Nombre ?? "—",
            t.IdAprendizNavigation?.IdUsuarioNavigation?.Documento.ToString() ?? "—",
            t.Fecha,
            t.FechaRetiro,
            t.EstadoReserva ?? "Pendiente",
            t.DetalleTransaccions
                .Select(d => new VentaItemDto(
                    d.IdProductoNavigation?.NombreProducto ?? "—",
                    d.Cantidad,
                    d.PrecioUnitario))
                .ToList(),
            t.Total
        )).ToList();
    }
}
