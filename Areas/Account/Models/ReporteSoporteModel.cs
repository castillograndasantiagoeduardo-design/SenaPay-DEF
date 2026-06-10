using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace SENA_PAY_PRUEBAS.Areas.Account.Models;

public sealed class ReporteSoporteModel
{
    [Required] public string TipoDocumento { get; set; } = string.Empty;

    [Required]
    public int Documento { get; set; } 

    [Required] public string Rol { get; set; } = string.Empty;

    [Required] public string TipoReporte { get; set; } = string.Empty;

    [Required, MinLength(10)]
    public string Descripcion { get; set; } = string.Empty;

    public IFormFile? Evidencia { get; set; }
}