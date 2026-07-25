using Microsoft.AspNetCore.Mvc;
using Scholar.Models;
using System.Diagnostics;

namespace Scholar.Controllers
{
    public class DashboardController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
