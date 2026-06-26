// SenaPay.Application/DTOs/Tienda/AsignarAdminRequest.cs
namespace SenaPay.Application.DTOs.Tienda;

public class AsignarAdminRequest
{
    public int IdTienda { get; set; }
    public int? IdAdminCafeteria { get; set; } // null = desasignar
}