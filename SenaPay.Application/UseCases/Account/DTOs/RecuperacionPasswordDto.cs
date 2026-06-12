namespace SenaPay.Application.UseCases.Account.DTOs;

public record ValidarAccesoRequest(int Documento, string Password, int IdRol);

public record ValidarAccesoResultado(bool Ok, string Mensaje, int IdRol, string DocumentoSesion);

public record RecuperarPasswordRequest(int Documento);

public record RecuperarPasswordResultado(bool Ok, string Mensaje);

public record VerificarCodigoResultado(bool Ok, string Token);

public record RestablecerPasswordRequest(string Token, string NuevaPassword, string ConfirmarPassword);

public record RestablecerPasswordResultado(bool Ok, string Mensaje);