using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Restaurant.Models;
using Restaurant.ViewModels;
using Restaurant.Services;

namespace Restaurant.Controllers
{
    public class GebruikerController : Controller
    {
        private readonly UserManager<CustomUser> _userManager;
        private readonly EmailService _emailService;

        public GebruikerController(
            UserManager<CustomUser> userManager,
            EmailService emailService)
        {
            _userManager = userManager;
            _emailService = emailService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult WachtwoordVergeten()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> WachtwoordVergeten(
            WachtwoordVergetenViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            CustomUser? gebruiker =
                await _userManager.FindByEmailAsync(model.Email);

            if (gebruiker == null)
            {
                TempData["ResetMelding"] =
                    "Als het e-mailadres bij ons bekend is, ontvang je een e-mail met een resetlink.";

                return RedirectToAction(nameof(WachtwoordVergeten));
            }

            string token =
                await _userManager.GeneratePasswordResetTokenAsync(gebruiker);

            string? resetLink = Url.Action(
                "WachtwoordResetten",
                "Gebruiker",
                new
                {
                    email = model.Email,
                    token = token
                },
                Request.Scheme
            );

            if (resetLink != null)
            {
                string onderwerp = "Wachtwoord opnieuw instellen";

                string bericht =
                    "<p>Je hebt gevraagd om je wachtwoord opnieuw in te stellen.</p>" +
                    "<p><a href=\"" + resetLink + "\">Klik hier om je wachtwoord opnieuw in te stellen</a></p>" +
                    "<p>Heb je dit niet aangevraagd? Dan mag je deze e-mail negeren.</p>";

                await _emailService.VerstuurEmailAsync(
                    model.Email,
                    onderwerp,
                    bericht
                );
            }

            TempData["ResetMelding"] =
                "Als het e-mailadres bij ons bekend is, ontvang je een e-mail met een resetlink.";

            return RedirectToAction(nameof(WachtwoordVergeten));
        }

        [HttpGet]
        public async Task<IActionResult> WachtwoordResetten(
            string email,
            string token)
        {
            if (string.IsNullOrEmpty(email) ||
                string.IsNullOrEmpty(token))
            {
                TempData["ResetFout"] =
                    "Deze resetlink is ongeldig of niet meer geldig.";

                return RedirectToAction(nameof(WachtwoordVergeten));
            }

            CustomUser? gebruiker =
                await _userManager.FindByEmailAsync(email);

            if (gebruiker == null)
            {
                TempData["ResetFout"] =
                    "Deze resetlink is ongeldig of niet meer geldig.";

                return RedirectToAction(nameof(WachtwoordVergeten));
            }

            bool tokenIsGeldig =
                await _userManager.VerifyUserTokenAsync(
                    gebruiker,
                    _userManager.Options.Tokens.PasswordResetTokenProvider,
                    UserManager<CustomUser>.ResetPasswordTokenPurpose,
                    token
                );

            if (!tokenIsGeldig)
            {
                TempData["ResetFout"] =
                    "Deze resetlink is verlopen of niet meer geldig.";

                return RedirectToAction(nameof(WachtwoordVergeten));
            }

            WachtwoordResettenViewModel model =
                new WachtwoordResettenViewModel
                {
                    Email = email,
                    Token = token
                };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> WachtwoordResetten(
            WachtwoordResettenViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            CustomUser? gebruiker =
                await _userManager.FindByEmailAsync(model.Email);

            if (gebruiker == null)
            {
                TempData["ResetFout"] =
                    "Deze resetlink is ongeldig of niet meer geldig.";

                return RedirectToAction(nameof(WachtwoordVergeten));
            }

            IdentityResult resultaat =
                await _userManager.ResetPasswordAsync(
                    gebruiker,
                    model.Token,
                    model.NieuwWachtwoord
                );

            if (!resultaat.Succeeded)
            {
                foreach (IdentityError fout in resultaat.Errors)
                {
                    ModelState.AddModelError(
                        string.Empty,
                        fout.Description
                    );
                }

                return View(model);
            }

            TempData["ResetSucces"] =
                "Je wachtwoord is succesvol gewijzigd.";

            return RedirectToAction(nameof(WachtwoordVergeten));
        }
    }
}
