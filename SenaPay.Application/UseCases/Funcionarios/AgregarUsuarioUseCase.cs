using SenaPay.Application.UseCases.Funcionarios.DTOs;
using SenaPay.Domain.Entities;
using SenaPay.Domain.Interfaces.Usuarios;

namespace SenaPay.Application.UseCases.Funcionarios;

public class AgregarUsuarioUseCase
{
    //_repo la variable que guarda el contrato de la interface IUsuarioRepository para poder usar los contratos de esa interface
    private readonly IUsuarioRepository _repo;

    //Constructor de IUsuarioRepository
    public AgregarUsuarioUseCase(IUsuarioRepository repo) => _repo = repo;

    public async Task<OperacionResultado> EjecutarAsync(AgregarUsuarioRequest request)
    {
        // ── Regla de negocio: documento único ──
        if (await _repo.ExisteDocumentoAsync(request.Documento))
            return new OperacionResultado(false, "Este número de documento ya está registrado.");

        // ── Crear usuario base ──
        var usuario = new Usuario
        {
            Documento = request.Documento,
            Clave = request.Documento.ToString(), // Contraseña inicial = documento
            IdRol = request.IdRol
        };
        await _repo.AgregarAsync(usuario);
        await _repo.GuardarCambiosAsync(); // Necesario para obtener IdUsuario generado

        // ── Crear perfil según rol ──
        if (request.IdRol == 1)
        {
            await _repo.AgregarAprendizAsync(new Aprendix
            {
                Nombre = request.Nombre,
                Correo = request.Correo,
                Telefono = request.Telefono,
                Saldo = request.Saldo,
                IdUsuario = usuario.IdUsuario
            });
        }
        else if (request.IdRol == 2)
        {
            await _repo.AgregarFuncionarioAsync(new Funcionario
            {
                Nombre = request.Nombre,
                Correo = request.Correo,
                Telefono = request.Telefono,
                Saldo = request.Saldo,
                IdUsuario = usuario.IdUsuario
            });
        }
        else if (request.IdRol == 3)
        {
            await _repo.AgregarAdminCafeteriaAsync(new AdminCafeterium
            {
                Nombre = request.Nombre,
                Correo = request.Correo,
                Telefono = request.Telefono,
                Saldo = request.Saldo,
                IdUsuario = usuario.IdUsuario
            });
        }

        await _repo.GuardarCambiosAsync();
        return new OperacionResultado(true, "Usuario y perfil creados correctamente.");
    }
}