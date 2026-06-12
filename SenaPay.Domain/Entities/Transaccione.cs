using System;
using System.Collections.Generic;

namespace SenaPay.Domain.Entities;

public partial class Transaccione
{
    public int IdTransaccion { get; set; }

    public decimal Total { get; set; }

    public DateTime Fecha { get; set; }

    public int IdAprendiz { get; set; }

    public virtual ICollection<DetalleTransaccion> DetalleTransaccions { get; set; } = new List<DetalleTransaccion>();

    public virtual Aprendix IdAprendizNavigation { get; set; } = null!;
    public virtual ICollection<ResultadoCompra> ResultadoCompras { get; set; } = new List<ResultadoCompra>();
}
