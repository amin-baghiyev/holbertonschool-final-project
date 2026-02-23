using Microsoft.AspNetCore.Mvc;

namespace PLDMS.PL.Controllers;

public class LoginController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}