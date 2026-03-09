
using FurnitureGardenDesign.Data.Models;
using FurnitureGardenDesign.Services.Core.Interfaces;
using FurnitureGardenDesign.Web.ViewModels.Account;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace FurnitureGardenDesign.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAccountService accountService;

        public AccountController(IAccountService accountService)
        {
            this.accountService = accountService;
        }


        
        [HttpGet]
        public IActionResult Register() => View();


        // Handles the registration form submission

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var result = await accountService.RegisterAsync(model);

            if (result.Success)
               
            return RedirectToAction("Index", "Home");

            foreach (var error in result.Errors)
                ModelState.AddModelError("", error);

            return View(model);
        }

        [HttpGet]
        public IActionResult Login() => View();


        // logs in the user and redirects to home page if successful, otherwise shows error message
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            { 
                TempData["Error"] = "Invalid login attempt.";
                return View(model); 
            }

            var success = await accountService.LoginAsync(model);

            if (success)
                
            return RedirectToAction("Index", "Home");

            ModelState.AddModelError("", "Invalid login attempt.");
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await accountService.LogoutAsync();
            return RedirectToAction("Index", "Home");
        }
    }

}
