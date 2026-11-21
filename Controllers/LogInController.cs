using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Restaurant.Data;

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
        public async Task<IActionResult> login(LogInVieuwModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            string rol = "";

            if (model.Email == "AllBytes" && model.Password == "NoBytesOwner2440")
            {
                rol = "Eigenaar";
            }
            else if (model.Email == "BigBytes" && model.Password == "NoBytesManager2440")
            {
                rol = "Manager";
            }
            else if (model.Email == "MediumBytes" && model.Password == "NoBytesWaiter2440")
            {
                rol = "Waiter";
            }
            else if (model.Email == "SmallBytes" && model.Password == "NoBytesCook2440")
            {
                rol = "Cook";
            }

            if(rol == "Eigenaar")
                {
                return RedirectToAction("index", "Owner");
                }
            else
            {
                return RedirectToAction("index", "Klant");
            }
        }
    }
}