namespace SenaPay.Domain.Entities;

public class Reporte
{
    public int Id_Reporte { get; set; }
    public string Radicado { get; set; } = string.Empty;
    public string Tipo_Reporte { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public string? Evidencia_Path { get; set; }
    public string Estado { get; set; } = "Pendiente";
    public DateTime Fecha_Creacion { get; set; } = DateTime.UtcNow;
    public DateTime? Fecha_Resolucion { get; set; }
    public int Id_Usuario { get; set; }

    // Navegación
    public Usuario? Usuario { get; set; }
}