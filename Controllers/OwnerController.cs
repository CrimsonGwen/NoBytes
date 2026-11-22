using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Restaurant.Controllers
{
    [Authorize(Roles = "Eigenaar")]
    public class OwnerController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
