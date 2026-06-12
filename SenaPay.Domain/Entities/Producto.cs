using System;
using System.Collections.Generic;

namespace SenaPay.Domain.Entities;

public partial class Producto
{
    public int IdProducto { get; set; }

    public string NombreProducto { get; set; } = null!;

    public decimal Precio { get; set; }

    public int Stock { get; set; }

    public string? Imagen { get; set; }

    public bool Estado { get; set; }

    public int? IdTienda { get; set; }

    public int? IdCategoria { get; set; }

    public virtual ICollection<DetalleTransaccion> DetalleTransaccions { get; set; } = new List<DetalleTransaccion>();

    public virtual Categoria? IdCategoriaNavigation { get; set; }

    public virtual Tiendum? IdTiendaNavigation { get; set; }
}
