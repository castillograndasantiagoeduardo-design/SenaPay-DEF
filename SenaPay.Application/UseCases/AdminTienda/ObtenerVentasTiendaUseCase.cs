using SenaPay.Application.DTOs.Venta;
using SenaPay.Domain.Interfaces;

namespace SenaPay.Application.UseCases.AdminTienda;

public class ObtenerVentasTiendaUseCase
{
    private readonly ITransaccionRepository _transaccionRepo;

    public ObtenerVentasTiendaUseCase(ITransaccionRepository transaccionRepo)
    {
        _transaccionRepo = transaccionRepo;
    }

    public async Task<List<VentaTiendaDto>> EjecutarAsync(int idTienda)
    {
        var transacciones = await _transaccionRepo.ObtenerPorTiendaAsync(idTienda);
        return transacciones.Select(t => new VentaTiendaDto(
            t.IdTransaccion,
            t.IdAprendizNavigation?.Nombre ?? "—",
            t.IdAprendizNavigation?.IdUsuarioNavigation?.Documento.ToString() ?? "—",
            t.Fecha,
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
