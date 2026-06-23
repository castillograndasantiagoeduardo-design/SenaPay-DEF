namespace SenaPay.Application.DTOs.Dashboard;

public class DashboardTiendaDto
{
    public string NombreTienda { get; set; } = null!;
    public string Ubicacion { get; set; } = null!;
    public string NombreSede { get; set; } = null!;
    public string CiudadSede { get; set; } = null!;
    public string NombreAdmin { get; set; } = null!;
    public decimal VentasHoy { get; set; }
    public decimal SaldoTienda { get; set; }
    public int TransaccionesHoy { get; set; }
    public int ProductosBajoStock { get; set; }
    public bool TieneCategorias { get; set; }
}