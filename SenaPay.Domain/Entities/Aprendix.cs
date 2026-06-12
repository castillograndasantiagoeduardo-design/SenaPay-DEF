using System;
using System.Collections.Generic;

namespace SenaPay.Domain.Entities;

public partial class Aprendix
{
    public int IdAprendiz { get; set; }

    public decimal Saldo { get; set; }

    public string Nombre { get; set; } = null!;

    public string Correo { get; set; } = null!;

    public string? Telefono { get; set; }

    public int IdUsuario { get; set; }

    public virtual Usuario IdUsuarioNavigation { get; set; } = null!;

    public virtual ICollection<Transaccione> Transacciones { get; set; } = new List<Transaccione>();
}
