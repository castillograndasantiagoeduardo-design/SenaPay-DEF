using SenaPay.Application.UseCases.Account.DTOs;
using SenaPay.Domain.Entities;
using SenaPay.Domain.Interfaces.Core;
using SenaPay.Domain.Interfaces.Usuarios;

namespace SenaPay.Application.UseCases.Account;

public class RecuperarPasswordUseCase
{
    private readonly IAccountRepository _repo;
    private readonly IEmailService _emailService;   // ← IEmailService, no EmailService

    public RecuperarPasswordUseCase(IAccountRepository repo, IEmailService emailService)
    {
        _repo = repo;
        _emailService = emailService;
    }

    public async Task<RecuperarPasswordResultado> EjecutarAsync(RecuperarPasswordRequest request)
    {
        var (usuario, correo) = await _repo.ObtenerUsuarioConCorreoAsync(request.Documento);

        if (usuario is null)
            return new RecuperarPasswordResultado(false, "El número de documento no está registrado.");

        if (string.IsNullOrEmpty(correo))
            return new RecuperarPasswordResultado(false, "No se encontró un correo asociado a este Usuario.");

        string token = new Random().Next(100000, 999999).ToString();

        await _repo.AgregarRecuperacionAsync(new RecuperacionPassword
        {
            IdUsuario = usuario.IdUsuario,
            Token = token,
            FechaExpiracion = DateTime.Now.AddMinutes(5),
            FechaCreacion = DateTime.Now,
            Usado = false
        });
        await _repo.GuardarCambiosAsync();

        string cuerpo = $"Hola, usa este token para recuperar tu clave: {token}";
        _emailService.EnviarCorreo(correo, "Recuperación de Contraseña - SENA-PAY", cuerpo);

        return new RecuperarPasswordResultado(true, string.Empty);
    }
}