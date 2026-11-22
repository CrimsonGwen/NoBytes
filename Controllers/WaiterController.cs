using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;

namespace Restaurant.Controllers
{
    [Authorize(Roles = "Waiter")]
    public class WaiterController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
