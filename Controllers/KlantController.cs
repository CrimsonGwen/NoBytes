namespace Restaurant.Controllers
{
    [Authorize(Roles = "Klant")]
    public class KlantController : Controller
    {
        private readonly UserManager<CustomUser> _userManager;
        private readonly SignInManager<CustomUser> _signInManager;
        private readonly IUnitOfWork _context;
        private readonly RestaurantContext _db;

        public KlantController(UserManager<CustomUser> userManager, SignInManager<CustomUser> signInManager, IUnitOfWork context, RestaurantContext db)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
            _db = db;
        }

        public IActionResult Index()
        {
            return View();
        }



        private async Task LoadLandenAsync(
            ManageAccountViewModel vm)
        {
            var landen = await _context.LandenRepository
                .GetAllAsync();

            vm.Landen = landen.Select(l => new SelectListItem
            {
                Value = l.Id.ToString(),
                Text = l.Naam
            });
        }
    }
}