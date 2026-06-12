using SenaPay.Application.UseCases.Funcionarios.DTOs;
using SenaPay.Domain.Entities;
using SenaPay.Domain.Interfaces;
using SenaPay.Domain.Interfaces.Core;
using SenaPay.Domain.Interfaces.Tienda;
using SenaPay.Domain.Interfaces.Usuarios;

namespace SenaPay.Application.UseCases.Funcionarios;

/// <summary>
/// Crea una Tienda nueva junto con su usuario AdminTienda (Rol 3)
/// en una sola operación atómica.
/// </summary>
public class CrearTiendaUseCase
{
    private readonly ITiendaRepository _tiendaRepo;
    private readonly IUsuarioRepository _usuarioRepo;

    public CrearTiendaUseCase(
        ITiendaRepository tiendaRepo,
        IUsuarioRepository usuarioRepo)
    {
        _tiendaRepo = tiendaRepo;
        _usuarioRepo = usuarioRepo;
    }

    public async Task<CrearTiendaResultado> EjecutarAsync(CrearTiendaRequest request)
    {
        // ── Regla 1: el documento del admin no puede estar duplicado ──
        if (await _usuarioRepo.ExisteDocumentoAsync(request.DocumentoAdmin))
            return new CrearTiendaResultado(false,
                "El documento del Administrador ya está registrado.");

        // ── Regla 2: crear el Usuario con Rol 3 ──────────────────────
        var nuevoUsuario = new Usuario
        {
            Documento = request.DocumentoAdmin,
            Clave = request.DocumentoAdmin.ToString(), // clave inicial = documento
            IdRol = 3
        };
        await _usuarioRepo.AgregarAsync(nuevoUsuario);
        await _usuarioRepo.GuardarCambiosAsync(); // necesario para obtener el Id generado

        // ── Regla 3: crear el perfil AdminCafeteria vinculado ────────
        var adminCafeteria = new AdminCafeterium
        {
            Nombre = request.NombreAdmin,
            Correo = request.CorreoAdmin,
            Telefono = request.TelefonoAdmin,
            IdUsuario = nuevoUsuario.IdUsuario
        };
        await _usuarioRepo.AgregarAdminCafeteriaAsync(adminCafeteria);
        await _usuarioRepo.GuardarCambiosAsync();

        // ── Regla 4: crear la Tienda vinculada al admin ───────────────
        var tienda = new Tiendum
        {
            Nombre = request.NombreTienda,
            Ubicacion = request.Ubicacion,
            IdSede = request.IdSede,
            IdAdminCafeteria = adminCafeteria.IdAdminCafeteria
        };
        await _tiendaRepo.CrearAsync(tienda);
        await _tiendaRepo.GuardarCambiosAsync();

        return new CrearTiendaResultado(true,
            $"Tienda '{request.NombreTienda}' creada correctamente.", tienda.IdTienda);
    }
}