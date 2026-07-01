namespace SenaPay.Domain.Entities;

public partial class MovimientoSaldo
{
    public int IdMovimiento { get; set; }
    public int IdUsuario { get; set; }
    public string TipoMovimiento { get; set; } = null!;   // "RECARGA" | "CONSUMO"
    public decimal Monto { get; set; }
    public decimal SaldoResultante { get; set; }
    public DateTime Fecha { get; set; }
    public string? Descripcion { get; set; }
    public int? IdAdminRealizador { get; set; }
    public int? IdTransaccion { get; set; }

    public virtual Usuario IdUsuarioNavigation { get; set; } = null!;
    public virtual AdminCafeterium? IdAdminRealizadorNavigation { get; set; }
    public virtual Transaccione? IdTransaccionNavigation { get; set; }
}
