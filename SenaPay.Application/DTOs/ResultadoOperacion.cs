// SenaPay.Application/DTOs/ResultadoOperacion.cs
namespace SenaPay.Application.DTOs;

public class ResultadoOperacion
{
    public bool Ok { get; set; }
    public string Mensaje { get; set; } = string.Empty;

    public static ResultadoOperacion Exito(string msg) => new() { Ok = true, Mensaje = msg };
    public static ResultadoOperacion Error(string msg) => new() { Ok = false, Mensaje = msg };
}