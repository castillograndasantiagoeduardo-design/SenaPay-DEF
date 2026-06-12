using System;
using System.Collections.Generic;

namespace SenaPay.Domain.Entities;

public partial class Sede
{
    public int IdSede { get; set; }

    public string Nombre { get; set; } = null!;

    public string Ciudad { get; set; } = null!;

    public bool? Estado { get; set; }

    public virtual ICollection<Tiendum> Tienda { get; set; } = new List<Tiendum>();

    public virtual ICollection<Usuario> Usuarios { get; set; } = new List<Usuario>();
}
