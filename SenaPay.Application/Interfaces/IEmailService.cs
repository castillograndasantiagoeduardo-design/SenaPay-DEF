namespace SenaPay.Application.Interfaces;

/// <summary>
/// Contrato de envío de correos.
/// La Aplicación solo conoce esta interfaz, nunca la implementación SMTP.
/// </summary>
public interface IEmailService
{
    void EnviarCorreo(string receptor, string asunto, string mensaje);
}