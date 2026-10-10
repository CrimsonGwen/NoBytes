using Microsoft.AspNetCore.Mvc;

namespace Restaurant.Controllers
{
    public class LogInController : Controller
    {
        public IActionResult LogIn()
        {
            return View();
        }
    }
}
