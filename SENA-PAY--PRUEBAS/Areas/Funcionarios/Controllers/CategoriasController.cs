// Areas/Funcionarios/Controllers/CategoriasController.cs
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SenaPay.Application.DTOs.Categorias;
using SenaPay.Application.UseCases.Categorias;

namespace SenaPay.Presentation.Areas.Funcionarios.Controllers;

[Area("Funcionarios")]
[Authorize(Roles = "2")]
public class CategoriasController : Controller
{
    private readonly CrearCategoriaUseCase _crearCategoria;
    private readonly ObtenerCategoriasUseCase _obtenerCategorias;
    private readonly EliminarCategoriaUseCase _eliminarCategoria;
    private readonly EditarCategoriaUseCase _editarCategoria;

    public CategoriasController(
        CrearCategoriaUseCase crearCategoria,
        ObtenerCategoriasUseCase obtenerCategorias,
        EliminarCategoriaUseCase eliminarCategoria,
        EditarCategoriaUseCase editarCategoria)
    {
        _crearCategoria = crearCategoria;
        _obtenerCategorias = obtenerCategorias;
        _eliminarCategoria = eliminarCategoria;
        _editarCategoria = editarCategoria;
    }

    // GET /Funcionarios/Categorias
    public async Task<IActionResult> Index()
    {
        ViewData["Title"] = "Categorías";
        ViewData["Breadcrumb"] = "Categorías";
        return View(await _obtenerCategorias.EjecutarAsync());
    }

    // POST /Funcionarios/Categorias/Crear
    [HttpPost]
    [Route("Funcionarios/Categorias/Crear")]
    public async Task<IActionResult> Crear([FromBody] CrearCategoriaRequest request)
    {
        if (!ModelState.IsValid)
        {
            var errores = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
            return BadRequest(new { mensaje = string.Join(" ", errores) });
        }

        var (exito, mensaje, categoria) = await _crearCategoria.EjecutarAsync(request);
        if (!exito) return Conflict(new { mensaje });

        return Ok(new { mensaje, categoria });
    }

    // PUT /Funcionarios/Categorias/Editar/5
    [HttpPut]
    [Route("Funcionarios/Categorias/Editar/{id:int}")]
    public async Task<IActionResult> Editar(int id, [FromBody] CrearCategoriaRequest request)
    {
        if (!ModelState.IsValid)
        {
            var errores = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
            return BadRequest(new { mensaje = string.Join(" ", errores) });
        }

        var (exito, mensaje, categoria) = await _editarCategoria.EjecutarAsync(id, request);
        if (!exito) return Conflict(new { mensaje });

        return Ok(new { mensaje, categoria });
    }

    // DELETE /Funcionarios/Categorias/Eliminar/5
    [HttpDelete]
    [Route("Funcionarios/Categorias/Eliminar/{id:int}")]
    public async Task<IActionResult> Eliminar(int id)
    {
        var (exito, mensaje) = await _eliminarCategoria.EjecutarAsync(id);
        if (!exito) return NotFound(new { mensaje });

        return Ok(new { mensaje });
    }
}