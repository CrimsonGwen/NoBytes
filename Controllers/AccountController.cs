using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Restaurant.Controllers
{
    public class AccountController : Controller
    {
        private readonly ILogger<AccountController> _logger;
        private readonly UserManager<CustomUser> _userManager;
        private readonly SignInManager<CustomUser> _signInManager;
        private readonly IUnitOfWork _context;
        private readonly IMapper _mapper;

        public AccountController(ILogger<AccountController> logger,UserManager<CustomUser> userManager, SignInManager<CustomUser> signInManager , IUnitOfWork context, IMapper mapper)
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
            var vm = await GetDataAsync();
            return View(vm);
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
                await _userManager.AddToRoleAsync(user, "klant");
                await _signInManager.SignInAsync(user, isPersistent: false);
                //return RedirectToAction("Create", "Reservatie");
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

            await _context.CustomUserRepository.AddAsync(user);
            await _context.SaveChangesAsync();

            return View("Confirmation", user);
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
    }
}
