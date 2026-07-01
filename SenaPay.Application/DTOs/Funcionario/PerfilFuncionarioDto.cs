namespace SenaPay.Application.DTOs.Funcionario;

public record PerfilFuncionarioDto(
    int IdFuncionario,
    string Nombre,
    decimal Saldo,
    string Correo,
    string? Telefono,
    string Documento,
    string Sede
);
