using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace SenaPay.Web.Areas.AdminCafeteria.Controllers;

[Area("AdminCafeteria")]
[Authorize(AuthenticationSchemes = "SenaPayCookies")]
public class VentasController : Controller
{
    public IActionResult Index() => View();
}