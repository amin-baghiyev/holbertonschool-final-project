using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace PLDMS.PL.Areas.Student.Controllers;

[Area("Student")]
[Authorize(Roles = "Student")]
public class DashboardController : Controller
{
    // GET
    public IActionResult Index()
    {
        return View();
    }
}