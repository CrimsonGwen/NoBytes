using Microsoft.AspNetCore.Mvc;

namespace Restaurant.Controllers
{
    public class OwnerController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
