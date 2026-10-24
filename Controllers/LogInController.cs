using Microsoft.AspNetCore.Mvc;
using Restaurant.Data;

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
