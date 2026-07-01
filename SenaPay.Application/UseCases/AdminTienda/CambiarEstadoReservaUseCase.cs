using SenaPay.Domain.Interfaces;

namespace SenaPay.Application.UseCases.AdminTienda;

public class CambiarEstadoReservaUseCase
{
    private static readonly string[] _estadosValidos = ["Pendiente", "Entregada", "Cancelada"];
    private readonly ITransaccionRepository _repo;

    public CambiarEstadoReservaUseCase(ITransaccionRepository repo) => _repo = repo;

    public async Task<(bool Ok, string Mensaje)> EjecutarAsync(
        int idTransaccion, string nuevoEstado, int idTienda)
    {
        if (!_estadosValidos.Contains(nuevoEstado))
            return (false, "Estado no válido.");

        await _repo.CambiarEstadoReservaAsync(idTransaccion, nuevoEstado, idTienda);
        return (true, $"Reserva #{idTransaccion} marcada como {nuevoEstado}.");
    }
}
