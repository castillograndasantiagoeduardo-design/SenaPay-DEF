using System;
using System.Collections.Generic;

namespace SenaPay.Domain.Entities;

public partial class RecuperacionPassword
{
    public int IdRecuperacion { get; set; }

    public string Token { get; set; } = null!;

    public DateTime FechaCreacion { get; set; }

    public DateTime FechaExpiracion { get; set; }

    public bool? Usado { get; set; }

    public int IdUsuario { get; set; }

    public virtual Usuario IdUsuarioNavigation { get; set; } = null!;
}
