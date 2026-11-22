using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;

namespace Restaurant.Controllers
{
    [Authorize(Roles = "Manager")]
    public class ManagerController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
