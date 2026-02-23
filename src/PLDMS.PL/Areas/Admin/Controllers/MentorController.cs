using Microsoft.AspNetCore.Mvc;

namespace PLDMS.PL.Areas.Admin.Controllers;

[Area("Admin")]
public class UserController : Controller
{
    // GET
    public IActionResult Index()
    {
        return View();
    }
}