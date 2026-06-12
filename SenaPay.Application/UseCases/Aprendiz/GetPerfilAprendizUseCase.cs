using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SenaPay.Domain.Interfaces.Usuarios;

namespace SenaPay.Application.UseCases.Aprendiz;

/// <summary>
/// Caso de uso: Obtener el perfil de un aprendiz a partir de su documento.
/// Contiene la lógica de negocio; no depende de EF Core ni de HttpContext.
/// </summary>

//Caso de uso para obtener el perfil del aprendiz
public class GetPerfilAprendizUseCase
{
    //Declara una variable privada para guardar el "enchufe" (el contrato)
    private readonly IAprendizRepository _aprendizRepo;

    public GetPerfilAprendizUseCase(IAprendizRepository aprendizRepo)
    {
        _aprendizRepo = aprendizRepo;
    }

    /// <summary>
    /// Ejecuta el caso de uso.
    /// </summary>
    /// <param name="documento">Número de documento del aprendiz.</param>
    /// <returns>DTO con los datos del perfil, o null si no existe.</returns>

    //Es el método que activa el caso de uso.
    //async y Task: Significan que el proceso es asíncrono (no va a bloquear la página web mientras busca en la base de datos).
    //<PerfilAprendizDto?>: Promete que cuando termine, va a devolver el record con los 4 datos limpios que vimos antes, o un valor nulo (?) si algo sale mal.
    public async Task<PerfilAprendizDto?> EjecutarAsync(int documento)
    {
        //Aca el caso de uso llama al contrato (_aprendizRepo) y le dice que busque en la BSD al aprendixz con ese documento
        var aprendiz = await _aprendizRepo.ObtenerPorDocumentoAsync(documento);

        //Si no lo encuentra retorna null y asi un mensaje de error
        if (aprendiz is null)
            return null;

        //Si lo encuentra crea el Perfil del Aprendiz cumpliendo el caso de uso y lo manda al controlador 
        return new PerfilAprendizDto(
            Nombre: aprendiz.Nombre,
            Saldo: aprendiz.Saldo,
            Correo: aprendiz.Correo,
            Ficha: "2827102"   // Reemplazar con aprendiz.Ficha cuando exista en BD
        );
    }
}
