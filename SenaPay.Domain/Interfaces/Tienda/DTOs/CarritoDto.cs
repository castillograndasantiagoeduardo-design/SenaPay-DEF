using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SenaPay.Domain.Interfaces.Tienda.DTOs;

public class CarritoDto
{
    public int Id_Producto { get; set; }
    public string NombreProducto { get; set; } = string.Empty;
    public decimal PrecioUnitario { get; set; }
    public int Cantidad { get; set; }
    public decimal Subtotal => PrecioUnitario * Cantidad;
    public List<ItemCarritoDto> Items { get; set; } = new();

    // Calculados — el controlador los usa para la vista
    public decimal Total => Items.Sum(i => i.Subtotal);
    public int TotalItems => Items.Sum(i => i.Cantidad);
}
