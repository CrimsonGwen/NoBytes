using Microsoft.AspNetCore.Mvc;

namespace Restaurant.Controllers
{
    public class CreatOrderController : Controller
    {
        private readonly ILogger<CreatOrderController> _logger;

        public CreatOrderController(ILogger<CreatOrderController> logger)
        {
            _logger = logger;
        }
        public IActionResult Index()
        {
            return View();
        }
    }
}
