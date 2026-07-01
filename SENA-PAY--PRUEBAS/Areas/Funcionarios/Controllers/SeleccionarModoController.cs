using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SENA_PAY__PRUEBAS.Filters;

namespace SENA_PAY__PRUEBAS.Areas.Funcionarios.Controllers;

[Area("Funcionarios")]
[Authorize(Roles = "2")]
[NoCache]
public class SeleccionarModoController : Controller
{
    // GET /Funcionarios/SeleccionarModo/Index
    public IActionResult Index() => View();
}
