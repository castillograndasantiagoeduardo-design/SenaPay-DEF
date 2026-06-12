using System;
using System.Collections.Generic;

namespace SenaPay.Domain.Entities;

public partial class Tiendum
{
    public int IdTienda { get; set; }

    public string Nombre { get; set; } = null!;

    public string Ubicacion { get; set; } = null!;

    public int IdAdminCafeteria { get; set; }

    public virtual AdminCafeterium IdAdminCafeteriaNavigation { get; set; } = null!;

    public virtual ICollection<Producto> Productos { get; set; } = new List<Producto>();
    public int? IdSede { get; set; }
    public virtual Sede IdSedeNavigation { get; set; } = null!;
}
