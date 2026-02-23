using Microsoft.AspNetCore.Mvc;

namespace PLDMS.PL.Areas.Admin.Controllers;

[Area("Admin")]
public class MentorController : Controller
{
    // GET
    public IActionResult Index()
    {
        return View();
    }
}