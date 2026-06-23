using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SenaPay.Application.UseCases.Aprendiz;

/// <summary>
/// Datos que el caso de uso devuelve hacia el controlador.
/// Solo expone lo que la vista necesita, sin exponer la entidad de dominio.
/// </summary>
public record PerfilAprendizDto(
    string Nombre,
    decimal Saldo,
    string Correo,
    string Ficha         // Estático por ahora; cuando tengas la columna en BD, se mapea aquí
);
