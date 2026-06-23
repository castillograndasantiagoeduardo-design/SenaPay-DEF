using SenaPay.Application.DTOs.Usuarios;
using SenaPay.Domain.Interfaces;

namespace SenaPay.Application.UseCases.Account;

public class ValidarAccesoUseCase
{
    private readonly IAccountRepository _repo;

    public ValidarAccesoUseCase(IAccountRepository repo) => _repo = repo;

    public async Task<ValidarAccesoResultado> EjecutarAsync(ValidarAccesoRequest request)
    {
        var usuario = await _repo.ObtenerUsuarioPorDocumentoYClaveAsync(
            request.Documento, request.Password);

        if (usuario is null)
            return new ValidarAccesoResultado(false, "Documento o contraseña incorrectos.",
                0, string.Empty, string.Empty,0, null, null);

        if (usuario.IdRol != request.IdRol)
            return new ValidarAccesoResultado(false, "El rol seleccionado no coincide con su cuenta.",
                0, string.Empty, string.Empty,0, null, null);

        // Datos del perfil según el rol
        string nombre = string.Empty;
        int? idTienda = null;
        int? idAdminCafeteria = null;

        switch (usuario.IdRol)
        {
            /////////
            case 3:
                // AdminCafeterium → busca la tienda asociada
                var admin = usuario.AdminCafeteria.FirstOrDefault();
                if (admin != null)
                {
                    idAdminCafeteria = admin.IdAdminCafeteria;
                    nombre = admin.Nombre;
                    var tienda = admin.Tienda.FirstOrDefault();
                    idTienda = tienda?.IdTienda;
                }
                break;

            case 2:
                var func = usuario.Funcionarios.FirstOrDefault();
                nombre = func?.Nombre ?? string.Empty;
                break;

            case 1:
                var aprendiz = usuario.Aprendices.FirstOrDefault();
                nombre = aprendiz?.Nombre ?? string.Empty;
                break;
        }

        return new ValidarAccesoResultado(
            Ok: true,
            Mensaje: string.Empty,
            IdRol: usuario.IdRol,
            DocumentoSesion: usuario.Documento.ToString(),
            Nombre: nombre,
            IdUsuario: usuario.IdUsuario,  // ← AGREGAR
            IdTienda: idTienda,
            IdAdminCafeteria: idAdminCafeteria
        );
    }
}