using System;
using System.Collections.Generic;

namespace SenaPay.Domain.Entities;

public partial class AdminCafeterium
{
    public int IdAdminCafeteria { get; set; }

    public string Nombre { get; set; } = null!;

    public string Correo { get; set; } = null!;

    public string Telefono { get; set; } = null!;
    public decimal Saldo { get; set; }

    public int IdUsuario { get; set; }

    public virtual Usuario IdUsuarioNavigation { get; set; } = null!;

    public virtual ICollection<Tiendum> Tienda { get; set; } = new List<Tiendum>();
}
