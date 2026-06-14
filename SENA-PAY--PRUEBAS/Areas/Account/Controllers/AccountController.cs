using Microsoft.AspNetCore.Mvc;
using SenaPay.Application.UseCases.Account;
using SenaPay.Application.UseCases.Account.DTOs;
using SENA_PAY_PRUEBAS.Areas.Account.Models;

namespace SENA_PAY__PRUEBAS.Areas.Account.Controllers;

[Area("Account")]
public class AccountController : Controller
{
    private readonly ValidarAccesoUseCase _validarAccesoUseCase;
    private readonly RecuperarPasswordUseCase _recuperarPasswordUseCase;
    private readonly VerificarCodigoUseCase _verificarCodigoUseCase;
    private readonly RestablecerPasswordUseCase _restablecerPasswordUseCase;
    private readonly CrearReporteUseCase _crearReporteUseCase;
    private readonly IWebHostEnvironment _env;          // ← NUEVO

    public AccountController(
        ValidarAccesoUseCase validarAccesoUseCase,
        RecuperarPasswordUseCase recuperarPasswordUseCase,
        VerificarCodigoUseCase verificarCodigoUseCase,
        RestablecerPasswordUseCase restablecerPasswordUseCase,
        CrearReporteUseCase crearReporteUseCase,
        IWebHostEnvironment env)
    {
        _validarAccesoUseCase = validarAccesoUseCase;
        _recuperarPasswordUseCase = recuperarPasswordUseCase;
        _verificarCodigoUseCase = verificarCodigoUseCase;
        _restablecerPasswordUseCase = restablecerPasswordUseCase;
        _crearReporteUseCase = crearReporteUseCase;
        _env = env;
    }

    public IActionResult Login() => View();

    // ── VALIDAR ACCESO ────────────────────────────────────────
    [HttpPost]
    public async Task<IActionResult> ValidarAcceso(string documento, string password, int idRol)
    {
        if (!int.TryParse(documento, out int docNum))
        {
            ViewBag.Mensaje = "El documento debe contener solo números.";
            return View("Login");
        }

        var resultado = await _validarAccesoUseCase.EjecutarAsync(
            new ValidarAccesoRequest(docNum, password, idRol));

        if (!resultado.Ok)
        {
            ViewBag.Mensaje = resultado.Mensaje;
            return View("Login");
        }

        // ── Sesión: responsabilidad de la capa de Presentación ──
        HttpContext.Session.SetString("UsuarioDoc", resultado.DocumentoSesion);

        return resultado.IdRol switch
        {
            // 1 => Redirecciona a la Acción "Perfil", del Controlador "PerfilAprendiz", en el Área "Usuarios"
            1 => RedirectToAction("Perfil", "PerfilAprendiz", new { area = "Usuarios" }),
            2 => RedirectToAction("AgregarUsuarios","Funcionarios", new { area = "Funcionarios" }),
            _ => View("Login")
        };
    }

    // ── RECUPERAR CONTRASEÑA ──────────────────────────────────
    [HttpGet]
    public IActionResult RecuperarPassword() => View();

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RecuperarPassword(string Documento)
    {
        if (string.IsNullOrEmpty(Documento))
        {
            ModelState.AddModelError("", "El número de documento es obligatorio.");
            return View();
        }

        if (!int.TryParse(Documento, out int docInt))
        {
            ModelState.AddModelError("", "El documento debe contener solo números.");
            return View();
        }

        var resultado = await _recuperarPasswordUseCase.EjecutarAsync(
            new RecuperarPasswordRequest(docInt));

        if (!resultado.Ok)
        {
            ModelState.AddModelError("", resultado.Mensaje);
            return View();
        }

        return View("CodVerificacion");
    }

    // ── VERIFICAR CÓDIGO ──────────────────────────────────────
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> VerificarCodigo(string Codigo)
    {
        var resultado = await _verificarCodigoUseCase.EjecutarAsync(Codigo);

        if (resultado.Ok)
            return RedirectToAction("RestablecerPassword", new { token = resultado.Token });

        ModelState.AddModelError("", "Código incorrecto o expirado.");
        return View("CodVerificacion");
    }

    // ── RESTABLECER CONTRASEÑA (GET) ──────────────────────────
    [HttpGet]
    public async Task<IActionResult> RestablecerPassword(string token)
    {
        var resultado = await _verificarCodigoUseCase.EjecutarAsync(token);

        if (!resultado.Ok)
        {
            TempData["Error"] = "El enlace ha expirado o ya fue utilizado.";
            return RedirectToAction("Login");
        }

        ViewBag.Token = token;
        return View();
    }

    // ── RESTABLECER CONTRASEÑA (POST) ─────────────────────────
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RestablecerPassword(
        string token, string nuevaPassword, string confirmarPassword)
    {
        var resultado = await _restablecerPasswordUseCase.EjecutarAsync(
            new RestablecerPasswordRequest(token, nuevaPassword, confirmarPassword));

        if (!resultado.Ok)
        {
            ViewBag.Error = resultado.Mensaje;
            ViewBag.Token = token;
            return View();
        }

        TempData["Mensaje"] = resultado.Mensaje;
        return RedirectToAction("Login");
    }

    // ── POST /Account/Account/EnviarReporte ──────────────────────────────
    [HttpPost]
    [ValidateAntiForgeryToken]
    //Desde el html en el formData llegan los datos a este controlador
    public async Task<IActionResult> EnviarReporte([FromForm] ReporteSoporteModel model)
    {
        // 1. Validar ModelState
        //Valida los [Required] de ReporteSoporteModel
        if (!ModelState.IsValid)
        {
            var errores = ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage);

            return Json(new { ok = false, msg = "Datos inválidos.", errores });
        }

        try
        {
            // 2. Manejar el archivo de evidencia (si se adjuntó)
            string? evidenciaPath = null;
            if (model.Evidencia is { Length: > 0 })
            {
                evidenciaPath = await GuardarEvidenciaAsync(model.Evidencia);
            }

            // 3. Ejecutar el caso de uso
            var dto = new CrearReporteDto(
                TipoDocumento: model.TipoDocumento,
                Documento: model.Documento,
                Rol: model.Rol,
                TipoReporte: model.TipoReporte,
                Descripcion: model.Descripcion,
                EvidenciaPath: evidenciaPath
            );

            //Luego de construirse el reporte y los datos se envian al_crearReporteUseCase con el dto
            string? radicado = await _crearReporteUseCase.EjecutarAsync(dto);

            if (radicado is null)
                return Json(new
                {
                    ok = false,
                    msg = "No encontramos un usuario con ese número de documento."
                });

            return Json(new
            {
                ok = true,
                msg = "Reporte enviado con éxito.",
                radicado
            });
        }
        catch (Exception ex)
        {
            // Log real aquí con ILogger
            return Json(new
            {
                ok = false,
                msg = "Error interno del servidor. Intenta de nuevo más tarde."
            });
        }
    }
    //----helpers----
    private async Task<string> GuardarEvidenciaAsync(IFormFile archivo)
    {
        // WebRootPath puede ser null en algunos entornos de desarrollo
        string webRoot = _env.WebRootPath
                      ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");

        string carpeta = Path.Combine(webRoot, "uploads", "reportes");
        Directory.CreateDirectory(carpeta); // crea uploads/reportes si no existe

        string extension = Path.GetExtension(archivo.FileName).ToLowerInvariant();
        string nombreUnico = $"{Guid.NewGuid()}{extension}";
        string rutaFisica = Path.Combine(carpeta, nombreUnico);

        await using var stream = System.IO.File.Create(rutaFisica);
        await archivo.CopyToAsync(stream);

        return $"/uploads/reportes/{nombreUnico}";
    }
    public IActionResult Soporte() => View();
}