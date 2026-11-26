using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Restaurant.Models;
using Restaurant.ViewModels;

namespace Restaurant.Controllers
{
    [Authorize(Roles = "Klant")]
    public class KlantController : Controller
    {
        private readonly UserManager<CustomUser> _userManager;
        private readonly SignInManager<CustomUser> _signInManager;
        private readonly IUnitOfWork _context;
        private readonly RestaurantContext _db;

        public KlantController(
            UserManager<CustomUser> userManager,
            SignInManager<CustomUser> signInManager,
            IUnitOfWork context,
            RestaurantContext db)
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

            await LoadLandenAsync(vm);

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ManageAccount(
            ManageAccountViewModel vm)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null || !user.Actief)
            {
                await _signInManager.SignOutAsync();
                return RedirectToAction("Login", "Account");
            }

            await LoadLandenAsync(vm);

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
                await _db.Database.BeginTransactionAsync();

            try
            {
                var roles = await _userManager.GetRolesAsync(user);

                if (roles.Count > 0)
                {
                    var roleResult = await _userManager
                        .RemoveFromRolesAsync(user, roles);

                    if (!roleResult.Succeeded)
                    {
                        await transaction.RollbackAsync();

                        TempData["ErrorMessage"] =
                            "De gebruikersrollen konden niet worden verwijderd.";

                        return RedirectToAction(nameof(ManageAccount));
                    }
                }

                // Persoonlijke gegevens anonimiseren.
                user.Voornaam = null;
                user.Achternaam = null;
                user.Adres = null;
                user.Huisnummer = null;
                user.Postcode = null;
                user.Gemeente = null;

                // Unieke, niet-persoonlijke gebruikersnaam.
                user.UserName = $"Deleted_{Guid.NewGuid():N}";
                user.NormalizedUserName =
                    user.UserName.ToUpperInvariant();

                user.Email = null;
                user.NormalizedEmail = null;
                user.EmailConfirmed = false;

                user.PhoneNumber = null;
                user.PhoneNumberConfirmed = false;

                // Oude wachtwoord en sessies ongeldig maken.
                user.PasswordHash = null;
                user.SecurityStamp = Guid.NewGuid().ToString();
                user.ConcurrencyStamp = Guid.NewGuid().ToString();
                user.TwoFactorEnabled = false;

                // Account deactiveren.
                user.Actief = false;
                user.LockoutEnabled = true;
                user.LockoutEnd = DateTimeOffset.MaxValue;
                user.AccessFailedCount = 0;

                _userManager.DeleteAsync(user);

                await _db.SaveChangesAsync();
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