using Microsoft.AspNetCore.Mvc;

namespace PLDMS.PL.Areas.Admin.Controllers;

public class Dashboard : Controller
{
    // GET
    public IActionResult Index()
    {
        return View();
    }
}