using SenaPay.Application.UseCases.Funcionarios.DTOs;
using SenaPay.Domain.Entities;
using SenaPay.Domain.Interfaces.Usuarios;

namespace SenaPay.Application.UseCases.Funcionarios;

public class EditarUsuarioUseCase
{
    //Linea para poder hablar con el IUsuarioRepository que tiene el contrato
    private readonly IUsuarioRepository _repo;
    public EditarUsuarioUseCase(IUsuarioRepository repo) => _repo = repo;

    public async Task<OperacionResultado> EjecutarAsync(EditarUsuarioRequest request)
    {
        bool actualizado = false;

        var aprendiz = await _repo.ObtenerAprendizPorIdAsync(request.IdUsuario);
        if (aprendiz is not null)
        {
            aprendiz.Nombre = request.Nombre;
            aprendiz.Correo = request.Correo;
            aprendiz.Telefono = request.Telefono;
            aprendiz.Saldo = request.Saldo;
            actualizado = true;
        }

        var funcionario = await _repo.ObtenerFuncionarioPorIdAsync(request.IdUsuario);
        if (funcionario is not null)
        {
            funcionario.Nombre = request.Nombre;
            funcionario.Correo = request.Correo;
            funcionario.Telefono = request.Telefono;
            funcionario.Saldo = request.Saldo;
            actualizado = true;
        }

        var admincafeteria = await _repo.ObtenerAdminCafeteriaPorIdAsync(request.IdUsuario);
        if (admincafeteria is not null)
        {
            admincafeteria.Nombre = request.Nombre;
            admincafeteria.Correo = request.Correo;
            admincafeteria.Telefono = request.Telefono;
            admincafeteria.Saldo = request.Saldo;
            actualizado = true;
        }

        if (!actualizado)
            return new OperacionResultado(false, "Usuario no encontrado.");

        await _repo.GuardarCambiosAsync();
        return new OperacionResultado(true, "Usuario actualizado correctamente.");
    }
}