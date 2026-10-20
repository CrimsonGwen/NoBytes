namespace Restaurant.Controllers
{
    public class ReservationController:Controller
    {
        private readonly ILogger<ReservationController> _logger;
        private readonly UserManager<CustomUser> _userManager;
        private readonly SignInManager<CustomUser> _signInManager;
        private readonly IUnitOfWork _context;
        private readonly IMapper _mapper;

        public ReservationController(ILogger<ReservationController> logger, UserManager<CustomUser> userManager, SignInManager<CustomUser> signInManager, IUnitOfWork context, IMapper mapper)
        {
            _logger = logger;
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            return View();
        }
    }
}
