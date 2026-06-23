using SenaPay.Application.DTOs.Tienda;
using SenaPay.Domain.Entities;
using SenaPay.Domain.Enums;
using SenaPay.Domain.Interfaces;

namespace SenaPay.Application.UseCases.Tienda;

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
        // ── Guard Clauses: datos de la tienda ──────────────────────────────
        if (string.IsNullOrWhiteSpace(request.NombreTienda))
            return new CrearTiendaResultado(false, "El nombre de la tienda es obligatorio.");

        if (string.IsNullOrWhiteSpace(request.Ubicacion))
            return new CrearTiendaResultado(false, "La ubicación es obligatoria.");

        if (request.IdSede <= 0)
            return new CrearTiendaResultado(false, "Debe seleccionar una sede válida.");

        // ── Resolver IdAdminCafeteria según el modo ────────────────────────
        // ✅ Reemplaza por if/else
        (int idAdminFinal, string? errorAdmin) resultado;

        if (request.ModoAdmin == ModoAsignacionAdmin.Existente)
            resultado = await ResolverAdminExistenteAsync(request);
        else if (request.ModoAdmin == ModoAsignacionAdmin.Nuevo)
            resultado = await CrearNuevoAdminAsync(request);
        else
            resultado = (0, "Modo de asignación no válido.");

        if (resultado.idAdminFinal <= 0)
            return new CrearTiendaResultado(false, resultado.errorAdmin!);
        // ── Crear la tienda ────────────────────────────────────────────────
        var tienda = new Tiendum
        {
            Nombre = request.NombreTienda,
            Ubicacion = request.Ubicacion,
            IdSede = request.IdSede,
            IdAdminCafeteria = resultado.idAdminFinal
        };

        await _tiendaRepo.CrearAsync(tienda);
        await _tiendaRepo.GuardarCambiosAsync();

        return new CrearTiendaResultado(
        true,
        $"Tienda '{request.NombreTienda}' creada correctamente.",
        tienda.IdTienda);
    }

    // ── Helpers privados ───────────────────────────────────────────────────
    private async Task<(int IdAdmin, string? Error)> ResolverAdminExistenteAsync(CrearTiendaRequest request)
    {
        if (request.IdAdminExistente is null or <= 0)
            return (0, "Debe seleccionar un administrador existente.");

        // ← CAMBIO: busca por PK propia, no por IdUsuario
        var admin = await _usuarioRepo
            .ObtenerAdminCafeteriaPorPkAsync(request.IdAdminExistente.Value);

        if (admin is null)
            return (0, "El administrador seleccionado no existe en la base de datos.");

        return (admin.IdAdminCafeteria, null);
    }

    private async Task<(int IdAdmin, string? Error)> CrearNuevoAdminAsync(
    CrearTiendaRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.NombreAdmin))
            return (0, "El nombre del administrador es obligatorio.");

        if (string.IsNullOrWhiteSpace(request.CorreoAdmin))
            return (0, "El correo del administrador es obligatorio.");

        if (request.DocumentoAdmin is null or <= 0)
            return (0, "El documento del administrador es obligatorio.");

        bool documentoOcupado = await _usuarioRepo
            .ExisteDocumentoAsync(request.DocumentoAdmin.Value);

        if (documentoOcupado)
            return (0, "El documento del administrador ya está registrado.");

        var nuevoUsuario = new Usuario
        {
            Documento = request.DocumentoAdmin.Value,
            Clave = request.DocumentoAdmin.Value.ToString(),
            IdRol = 3
        };

        await _usuarioRepo.AgregarAsync(nuevoUsuario);
        await _usuarioRepo.GuardarCambiosAsync();

        var adminCafeteria = new AdminCafeterium
        {
            Nombre = request.NombreAdmin,
            Correo = request.CorreoAdmin,
            Telefono = request.TelefonoAdmin ?? string.Empty,
            Saldo = 0,
            IdUsuario = nuevoUsuario.IdUsuario
        };

        await _usuarioRepo.AgregarAdminCafeteriaAsync(adminCafeteria);
        await _usuarioRepo.GuardarCambiosAsync();

        return (adminCafeteria.IdAdminCafeteria, null); // ← tupla consistente
    }
}