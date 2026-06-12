using System;
using System.Collections.Generic;

namespace SenaPay.Domain.Entities;

public partial class ResultadoCompra
{
    public int IdResultadoCompra { get; set; }

    public bool Exitosa { get; set; }

    public string Mensaje { get; set; } = null!;

    public decimal SaldoRestante { get; set; }

    public int IdTransaccion { get; set; }

    public virtual Transaccione IdTransaccionNavigation { get; set; } = null!;
    // Fábrica estática para resultados de éxito
    public static ResultadoCompra Ok(int idTransaccion, string mensaje) =>
        new() { Exitosa = true, IdTransaccion = idTransaccion, Mensaje = mensaje };

    // Fábrica estática para resultados de error
    public static ResultadoCompra Fallo(string mensaje) =>
        new() { Exitosa = false, Mensaje = mensaje };
}
