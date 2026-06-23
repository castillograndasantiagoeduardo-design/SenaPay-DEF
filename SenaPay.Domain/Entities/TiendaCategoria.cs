namespace SenaPay.Domain.Entities;

public class TiendaCategoria
{
    public int IdTienda { get; set; }
    public int IdCategoria { get; set; }
    public bool Habilitada { get; set; } = true;

    public virtual Tiendum? IdTiendaNavigation { get; set; }
    public virtual Categoria? IdCategoriaNavigation { get; set; }
}