using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

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
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
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

        [HttpGet]
        public async Task<IActionResult> ManageAccount()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null || !user.Actief)
            {
                await _signInManager.SignOutAsync();
                return RedirectToAction("Login", "Account");
            }

            var vm = new ManageAccountViewModel
            {
                Voornaam = user.Voornaam ?? "",
                Achternaam = user.Achternaam ?? "",
                Gebruikersnaam = user.UserName ?? "",
                Email = user.Email ?? "",
                Adres = user.Adres ?? "",
                Huisnummer = user.Huisnummer ?? "",
                Postcode = user.Postcode ?? "",
                Gemeente = user.Gemeente ?? "",
                LandId = user.LandId
            };

            await _context.LandenRepository.GetAllAsync();

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ManageAccount(ManageAccountViewModel vm)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null || !user.Actief)
            {
                await _signInManager.SignOutAsync();
                return RedirectToAction("Login", "Account");
            }

            await _context.LandenRepository.GetAllAsync();

            if (!ModelState.IsValid)
            {
                return View(vm);
            }

            var land = await _context.LandenRepository
                .GetByIdAsync(vm.LandId);

            if (land == null)
            {
                ModelState.AddModelError(
                    nameof(vm.LandId),
                    "Het geselecteerde land bestaat niet.");

                return View(vm);
            }

            var existingEmail = await _userManager
                .FindByEmailAsync(vm.Email.Trim());

            if (existingEmail != null && existingEmail.Id != user.Id)
            {
                ModelState.AddModelError(
                    nameof(vm.Email),
                    "Dit e-mailadres is al in gebruik.");

                return View(vm);
            }

            var existingUsername = await _userManager
                .FindByNameAsync(vm.Gebruikersnaam.Trim());

            if (existingUsername != null &&
                existingUsername.Id != user.Id)
            {
                ModelState.AddModelError(
                    nameof(vm.Gebruikersnaam),
                    "Deze gebruikersnaam is al in gebruik.");

                return View(vm);
            }

            user.Voornaam = vm.Voornaam.Trim();
            user.Achternaam = vm.Achternaam.Trim();
            user.UserName = vm.Gebruikersnaam.Trim();

            if (!string.Equals(
                user.Email,
                vm.Email.Trim(),
                StringComparison.OrdinalIgnoreCase))
            {
                user.EmailConfirmed = false;
            }

            user.Email = vm.Email.Trim();
            user.Adres = vm.Adres.Trim();
            user.Huisnummer = vm.Huisnummer.Trim();
            user.Postcode = vm.Postcode.Trim();
            user.Gemeente = vm.Gemeente.Trim();
            user.LandId = vm.LandId;

            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(
                        string.Empty,
                        error.Description);
                }

                return View(vm);
            }

            await _signInManager.RefreshSignInAsync(user);

            TempData["SuccessMessage"] =
                "Je accountgegevens zijn succesvol aangepast.";

            return RedirectToAction(nameof(ManageAccount));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteAccount()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null || !user.Actief)
            {
                await _signInManager.SignOutAsync();
                return RedirectToAction("Login", "Account");
            }

            await using var transaction =
                await _context.BeginTransactionAsync();

            try
            {
                var roles = await _userManager.GetRolesAsync(user);
                var rolelist = roles.ToList();

                foreach (var role in rolelist)
                {
                    await _userManager.RemoveFromRoleAsync(user, role);
                }

                var result = await _userManager.DeleteAsync(user);

                if (!result.Succeeded)
                {
                    await transaction.RollbackAsync();

                    TempData["ErrorMessage"] =
                        "De gebruikersrollen konden niet worden verwijderd.";

                    return RedirectToAction(nameof(ManageAccount));
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();

                TempData["ErrorMessage"] =
                    "Het account kon niet worden verwijderd.";

                return RedirectToAction(nameof(ManageAccount));
            }

            await _signInManager.SignOutAsync();

            return RedirectToAction("Index", "Home");
        }
    }
}
