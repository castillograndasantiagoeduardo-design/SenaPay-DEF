using SenaPay.Domain.Interfaces;
using System.Globalization;

namespace SenaPay.Application.UseCases.AdminTienda;

public class RecargaSaldoUseCase
{
    private readonly IAprendizRepository _aprendizRepo;

    public RecargaSaldoUseCase(IAprendizRepository aprendizRepo)
    {
        _aprendizRepo = aprendizRepo;
    }

    public async Task<(bool Ok, string Mensaje, decimal? NuevoSaldo, string? NombreAprendiz)> EjecutarAsync(
        int documentoAprendiz,
        decimal monto,
        string nombreAdmin,
        int idUsuarioAdmin)
    {
        var ci = new CultureInfo("es-CO");

        if (monto <= 0 || monto > 1_000_000)
            return (false, "El monto debe estar entre $1 y $1.000.000.", null, null);

        var aprendiz = await _aprendizRepo.ObtenerPorDocumentoAsync(documentoAprendiz);
        if (aprendiz == null)
            return (false, "No se encontró ningún aprendiz con ese documento.", null, null);

        var descripcion = $"Recarga realizada por {nombreAdmin}";
        var nuevoSaldo = await _aprendizRepo.RecargarSaldoAtomicoAsync(
            aprendiz.IdAprendiz,
            aprendiz.IdUsuarioNavigation.IdUsuario,
            monto,
            descripcion);

        return (true,
            $"Recarga de {monto.ToString("C0", ci)} realizada. Nuevo saldo: {nuevoSaldo.ToString("C0", ci)}.",
            nuevoSaldo,
            aprendiz.Nombre);
    }
}
