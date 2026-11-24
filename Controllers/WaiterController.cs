using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;

namespace Restaurant.Controllers
{
    [Authorize(Roles = "Ober")]
    public class WaiterController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
