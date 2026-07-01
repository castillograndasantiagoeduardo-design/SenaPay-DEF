using SenaPay.Application.DTOs.Dashboard;
using SenaPay.Domain.Interfaces;

namespace SenaPay.Application.UseCases.AdminTienda;

public class ObtenerDashboardTiendaUseCase
{
    private readonly IProductoRepository _productoRepo;
    private readonly ITiendaRepository _tiendaRepo;
    private readonly ITiendaCategoriaRepository _tiendaCatRepo;
    private readonly ITransaccionRepository _transaccionRepo;

    public ObtenerDashboardTiendaUseCase(
        IProductoRepository productoRepo,
        ITiendaRepository tiendaRepo,
        ITiendaCategoriaRepository tiendaCatRepo,
        ITransaccionRepository transaccionRepo)
    {
        _productoRepo = productoRepo;
        _tiendaRepo = tiendaRepo;
        _tiendaCatRepo = tiendaCatRepo;
        _transaccionRepo = transaccionRepo;
    }

    public async Task<DashboardTiendaDto> EjecutarAsync(int idTienda, string nombreAdmin)
    {
        var tienda = await _tiendaRepo.ObtenerPorIdAsync(idTienda)
            ?? throw new Exception("Tienda no encontrada.");

        var bajoStock = await _productoRepo.ContarBajoStockAsync(idTienda);
        var tieneCats = await _tiendaCatRepo.TieneCategoriasAsync(idTienda);
        var ventasHoy = await _transaccionRepo.ObtenerTotalVentasHoyAsync(idTienda);
        var transHoy  = await _transaccionRepo.ObtenerConteoTransaccionesHoyAsync(idTienda);

        return new DashboardTiendaDto
        {
            NombreTienda       = tienda.Nombre,
            NombreAdmin        = nombreAdmin,
            Ubicacion          = tienda.Ubicacion,
            NombreSede         = tienda.IdSedeNavigation?.Nombre ?? "Sin sede",
            CiudadSede         = tienda.IdSedeNavigation?.Ciudad ?? "",
            VentasHoy          = ventasHoy,
            SaldoTienda        = tienda.TotalIngresos,
            TotalIngresos      = tienda.TotalIngresos,
            TransaccionesHoy   = transHoy,
            ProductosBajoStock = bajoStock,
            TieneCategorias    = tieneCats
        };
    }
}