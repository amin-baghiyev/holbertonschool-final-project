using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace PLDMS.PL.Areas.Mentor.Controllers;

[Area("Mentor")]
[Authorize(Roles = "Mentor")]
public class SessionController : Controller
{
    [HttpGet]
    public IActionResult Index()
    {
        return View();
    }
}