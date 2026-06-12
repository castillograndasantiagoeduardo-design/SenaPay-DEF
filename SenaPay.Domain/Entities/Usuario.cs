using System;
using System.Collections.Generic;

namespace SenaPay.Domain.Entities;

public partial class Usuario
{
    public int IdUsuario { get; set; }

    public int Documento { get; set; }

    public string Clave { get; set; } = null!;

    public int IdRol { get; set; }

    public virtual ICollection<AdminCafeterium> AdminCafeteria { get; set; } = new List<AdminCafeterium>();

    public virtual ICollection<Aprendix> Aprendices { get; set; } = new List<Aprendix>();

    public virtual ICollection<Funcionario> Funcionarios { get; set; } = new List<Funcionario>();

    public virtual Role IdRolNavigation { get; set; } = null!;
    public int? IdSede { get; set; }
    public virtual Sede? IdSedeNavigation { get; set; } = null!;

    public virtual ICollection<RecuperacionPassword> RecuperacionPasswords { get; set; } = new List<RecuperacionPassword>();
}
