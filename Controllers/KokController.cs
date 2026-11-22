using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;

namespace Restaurant.Controllers
{
    public class KokController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
