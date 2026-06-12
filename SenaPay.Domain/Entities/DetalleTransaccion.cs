using System;
using System.Collections.Generic;

namespace SenaPay.Domain.Entities;

public partial class DetalleTransaccion
{
    public int IdDetalle { get; set; }

    public int Cantidad { get; set; }

    public decimal PrecioUnitario { get; set; }

    public int IdTransaccion { get; set; }

    public int IdProducto { get; set; }

    public virtual Producto IdProductoNavigation { get; set; } = null!;

    public virtual Transaccione IdTransaccionNavigation { get; set; } = null!;
}
