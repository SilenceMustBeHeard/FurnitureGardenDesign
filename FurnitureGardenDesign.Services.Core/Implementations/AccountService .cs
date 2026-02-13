using FurnitureGardenDesign.Data.Models;
using FurnitureGardenDesign.Services.Core.Interfaces;
using FurnitureGardenDesign.Web.ViewModels.Account;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace FurnitureGardenDesign.Services.Core.Implementations
{
    public class AccountService : IAccountService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;

        public AccountService(
            UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }


        // Returns a tuple indicating success and any error messages
        // This allows the caller to handle errors  without exceptions
        public async Task<(bool Success, IEnumerable<string> Errors)> RegisterAsync(RegisterViewModel model)
        {
            var user = new AppUser
            {
                UserName = model.Email,
                Email = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName,
                Address = model.Address
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
                return (false, result.Errors.Select(e => e.Description));

            if (!await _userManager.IsInRoleAsync(user, "User"))
            {
                await _userManager.AddToRoleAsync(user, "User");
            }

            await _signInManager.SignInAsync(user, false);

            return (true, Enumerable.Empty<string>());
        }


        // Returns true if login is successful, false otherwise
        // This method does not throw exceptions for failed logins

        public async Task<bool> LoginAsync(LoginViewModel model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
                return false;

            var result = await _signInManager.PasswordSignInAsync(
                user, model.Password, model.RememberMe, false);

            return result.Succeeded;
        }

        public async Task LogoutAsync()
        {
            await _signInManager.SignOutAsync();
        }
    }

}
