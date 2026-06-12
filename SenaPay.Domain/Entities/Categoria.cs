using System;
using System.Collections.Generic;

namespace SenaPay.Domain.Entities;

public partial class Categoria
{
    public int Id_Categoria { get; set; }

    public string? Nombre_Categoria { get; set; }

    public virtual ICollection<Producto> Productos { get; set; } = new List<Producto>();

    // Constructor sin parámetros requerido por EF Core
    private Categoria() { }

    // Factory method — garantiza que siempre se cree con nombre válido
    public static Categoria Crear(string nombre)
    {
        if (string.IsNullOrWhiteSpace(nombre))
            throw new ArgumentException("El nombre de la categoría no puede estar vacío.", nameof(nombre));

        return new Categoria { Nombre_Categoria = nombre.Trim() };
    }

    public void ActualizarNombre(string nuevoNombre)
    {
        if (string.IsNullOrWhiteSpace(nuevoNombre))
            throw new ArgumentException("El nombre no puede estar vacío.", nameof(nuevoNombre));

        Nombre_Categoria = nuevoNombre.Trim();
    }
}
