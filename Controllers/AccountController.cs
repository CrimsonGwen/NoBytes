namespace Restaurant.Controllers
{
    public class AccountController : Controller
    {
        private readonly ILogger<AccountController> _logger;
        private readonly UserManager<CustomUser> _userManager;
        private readonly SignInManager<CustomUser> _signInManager;
        private readonly IUnitOfWork _context;
        private readonly IMapper _mapper;

        public AccountController(ILogger<AccountController> logger, UserManager<CustomUser> userManager, SignInManager<CustomUser> signInManager, IUnitOfWork context, IMapper mapper)
        {
            _logger = logger;
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
            _mapper = mapper;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var vm = await GetDataAsync();
            return View(vm);
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View(new LogInViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateAccount(CreateAccountViewModel vm)
        {
            var landen = await _context.LandenRepository.GetAllAsync();

            if (!ModelState.IsValid)
            {
                vm.Landen = landen.Select(l => new SelectListItem
                {
                    Value = l.Id.ToString(),
                    Text = l.Naam
                });

                return View("Index", vm);
            }

            var user = _mapper.Map<CustomUser>(vm);

            user.Land = await _context.LandenRepository.GetByIdAsync(vm.LandId);
            user.Actief = true;
            user.UserName = vm.Gebruikersnaam;

            var result = await _userManager.CreateAsync(user, vm.Wachtwoord);

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, "Klant");
                await _signInManager.SignInAsync(user, isPersistent: false);
                return RedirectToAction("Index", "Reservation");
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }

                vm.Landen = landen.Select(l => new SelectListItem
                {
                    Value = l.Id.ToString(),
                    Text = l.Naam
                });

                return View("Index", vm);
            }

        }

        private async Task<CreateAccountViewModel> GetDataAsync()
        {
            var landen = await _context.LandenRepository.GetAllAsync();

            return new CreateAccountViewModel
            {
                Landen = landen.Select(l => new SelectListItem
                {
                    Value = l.Id.ToString(),
                    Text = l.Naam
                })
            };
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LogInViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.FindByEmailAsync(model.Email.Trim());

            if (user is null)
            {
                ModelState.AddModelError(string.Empty, "Account niet gevonden");
                ViewBag.ShowRegisterOption = true;
                return View(model);
            }

            if (!user.Actief)
            {
                ModelState.AddModelError(string.Empty, "Dit account is gedeactiveerd.");
                return View(model);
            }

            var passwordValid = await _userManager.CheckPasswordAsync(user, model.Password);
            if (!passwordValid)
            {
                ModelState.AddModelError(string.Empty, "Onjuist wachtwoord");
                return View(model);
            }

            await _signInManager.SignInAsync(user, isPersistent: false);

            var roles = await _userManager.GetRolesAsync(user);
            var role = roles.FirstOrDefault();

            return role switch
            {
                "Eigenaar" => RedirectToAction("Index", "Owner"),
                "Zaalverantwoordelijke" => RedirectToAction("Index", "Manager"),
                "Kok" => RedirectToAction("Index", "Cook"),
                "Ober" => RedirectToAction("Index", "Waiter"),
                "Klant" => RedirectToAction("Index", "Klant"),
                _ => RedirectToAction(nameof(Login))
            };
        }
    }
}
