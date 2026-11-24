using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Restaurant.Data;
using System.Security.Claims;
using Restaurant.Models;

namespace Restaurant.Controllers
{
    public class LogInController : Controller
    {
        [HttpGet]
        public IActionResult LogIn()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LogInVieuwModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            string? rol = null;


            if (model.Gebruikersnaam == "AllBytes" && model.Password == "NoBytesOwner2440")
            {
                rol = "Eigenaar";
            }
            else if (model.Gebruikersnaam == "BigBytes" && model.Password == "NoBytesManager2440")
            {
                rol = "Manager";
            }
            else if (model.Gebruikersnaam == "MediumBytes" && model.Password == "NoBytesWaiter2440")
            {
                rol = "Waiter";
            }
            else if (model.Gebruikersnaam == "SmallBytes" && model.Password == "NoBytesCook2440")
            {
                rol = "Cook";
            }

            if (rol == null) 
            {
                ModelState.AddModelError
                    ("", "Ongeldige gebruikersnaam of wachtwoord.");
                return View(model);
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, model.Gebruikersnaam),
                new Claim(ClaimTypes.Role, rol)
            };

            var identity = new ClaimsIdentity(
                claims, CookieAuthenticationDefaults.AuthenticationScheme
            );

            var pricipal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, pricipal);

            switch (rol)
            {
                case "Eigenaar":
                    return RedirectToAction("Index", "Owner");

                case "Manager":
                    return RedirectToAction("Index", "Manager");

                case "Cook":
                    return RedirectToAction("Index", "Kok");

                case "Waiter":
                    return RedirectToAction("Index", "Waiter");

                case "Klant":
                    return RedirectToAction("Index", "Klant");

                default:
                    return RedirectToAction("Login");
            }

        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(
                CookieAuthenticationDefaults.AuthenticationScheme
                );
            return RedirectToAction("Login");
        }

    }
}