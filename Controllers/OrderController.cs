namespace Restaurant.Controllers
{

    public class OrderController : Controller
    {
        private readonly IUnitOfWork _context;
        public OrderController(IUnitOfWork context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Bestellen()
        {
            var producten = await _context.ProductRepository.GetAllAsync();
            return View(producten);
        }
    }
}
