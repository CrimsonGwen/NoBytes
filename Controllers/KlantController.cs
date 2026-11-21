using Microsoft.AspNetCore.Mvc;

namespace Restaurant.Controllers
{
    public class KlantController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
