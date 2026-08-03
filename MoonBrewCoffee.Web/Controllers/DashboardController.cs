using Microsoft.AspNetCore.Mvc;

namespace MoonBrewCoffee.Infrastructure.Controllers
{
    public class DashboardController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}