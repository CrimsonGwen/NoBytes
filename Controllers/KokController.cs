using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;

namespace Restaurant.Controllers
{
    [Authorize(Roles = "Kok")]
    public class KokController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
