using SenaPay.Application.DTOs.Dashboard;
using SenaPay.Domain.Interfaces;

namespace SenaPay.Application.UseCases.AdminTienda;

public class ObtenerDashboardTiendaUseCase
{
    private readonly IProductoRepository _productoRepo;
    private readonly ITiendaRepository _tiendaRepo;
    private readonly ITiendaCategoriaRepository _tiendaCatRepo;

    public ObtenerDashboardTiendaUseCase(
        IProductoRepository productoRepo,
        ITiendaRepository tiendaRepo,
        ITiendaCategoriaRepository tiendaCatRepo)
    {
        _productoRepo = productoRepo;
        _tiendaRepo = tiendaRepo;
        _tiendaCatRepo = tiendaCatRepo;
    }

    public async Task<DashboardTiendaDto> EjecutarAsync(int idTienda, string nombreAdmin)
    {
        var tienda = await _tiendaRepo.ObtenerPorIdAsync(idTienda)
            ?? throw new Exception("Tienda no encontrada.");

        var bajoStock = await _productoRepo.ContarBajoStockAsync(idTienda);
        var tieneCats = await _tiendaCatRepo.TieneCategoriasAsync(idTienda);

        // Ventas y transacciones: extiende con tu repositorio de Transacciones
        return new DashboardTiendaDto
        {
            NombreTienda = tienda.Nombre,
            NombreAdmin = nombreAdmin,
            Ubicacion = tienda.Ubicacion,
            NombreSede = tienda.IdSedeNavigation?.Nombre ?? "Sin sede",
            CiudadSede = tienda.IdSedeNavigation?.Ciudad ?? "",
            VentasHoy = 0,          // TODO: conectar TransaccionRepository
            SaldoTienda = 0,        // TODO: conectar lógica de saldo
            TransaccionesHoy = 0,   // TODO
            ProductosBajoStock = bajoStock,
            TieneCategorias = tieneCats
        };
    }
}