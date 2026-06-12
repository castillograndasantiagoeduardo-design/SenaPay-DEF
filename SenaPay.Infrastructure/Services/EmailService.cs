using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;
using SenaPay.Domain.Interfaces.Core;

namespace SenaPay.Infrastructure.Services;

/// <summary>
/// Implementación SMTP concreta. Vive en Infraestructura porque
/// depende de System.Net.Mail y de configuración externa.
/// </summary>
public class EmailService : IEmailService
{
    private readonly IConfiguration _config;

    public EmailService(IConfiguration config)
    {
        _config = config;
    }

    public void EnviarCorreo(string receptor, string asunto, string mensaje)
    {
        var smtpServer = _config["EmailSettings:SmtpServer"];
        var smtpPort = int.Parse(_config["EmailSettings:SmtpPort"]!);
        var senderEmail = _config["EmailSettings:SenderEmail"];
        var senderName = _config["EmailSettings:SenderName"];
        var appPassword = _config["EmailSettings:AppPassword"];

        var client = new SmtpClient(smtpServer, smtpPort)
        {
            Credentials = new NetworkCredential(senderEmail, appPassword),
            EnableSsl = true
        };

        var mailMessage = new MailMessage
        {
            From = new MailAddress(senderEmail!, senderName),
            Subject = asunto,
            Body = mensaje,
            IsBodyHtml = true
        };

        mailMessage.To.Add(receptor);
        client.Send(mailMessage);
    }
}