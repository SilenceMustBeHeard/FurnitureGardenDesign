using FurnitureGardenDesign.Data.Models;
using FurnitureGardenDesign.Services.Core.Interfaces.Account;
using FurnitureGardenDesign.Web.ViewModels.Account;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace FurnitureGardenDesign.Services.Core.Implementations.Account
{
    public class AccountService : IAccountService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly IEmailService _emailService;
        private readonly ILogger<AccountService> _logger;

        public AccountService(
            UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager,
            IEmailService emailService,
            ILogger<AccountService> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailService = emailService;
            _logger = logger;
        }

        public async Task<(bool Success, string[] Errors)> RegisterAsync(RegisterViewModel model)
        {
            var user = new AppUser
            {
                UserName = model.Email,
                Email = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, "User");
                await _signInManager.SignInAsync(user, isPersistent: false);
                return (true, Array.Empty<string>());
            }

            return (false, result.Errors.Select(e => e.Description).ToArray());
        }

        public async Task<bool> LoginAsync(LoginViewModel model)
        {
            var result = await _signInManager.PasswordSignInAsync(
                model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);

            return result.Succeeded;
        }

        public async Task LogoutAsync()
        {
            await _signInManager.SignOutAsync();
        }

        public async Task<bool> ForgotPasswordAsync(string email, string resetLink)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null) return false;

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var fullResetLink = $"{resetLink}?token={Uri.EscapeDataString(token)}&email={Uri.EscapeDataString(email)}";

            var emailBody = GeneratePasswordResetEmailBody(user, fullResetLink);

            return await _emailService.SendEmailAsync(email, "Password Reset Request", emailBody);
        }

        public async Task<(bool Success, string[] Errors)> ResetPasswordAsync(ResetPasswordViewModel model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                return (false, new[] { "User not found." });
            }

            var result = await _userManager.ResetPasswordAsync(user, model.Token, model.Password);

            if (result.Succeeded)
            {
                return (true, Array.Empty<string>());
            }

            return (false, result.Errors.Select(e => e.Description).ToArray());
        }

        private string GeneratePasswordResetEmailBody(AppUser user, string resetLink)
        {
            return $@"
                <html>
                <head>
                    <style>
                        body {{ font-family: Arial, sans-serif; }}
                        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
                        .header {{ background: #c09a6c; padding: 20px; text-align: center; border-radius: 10px 10px 0 0; }}
                        .content {{ padding: 30px; background: #f9f9f9; color: #333; }}
                        .button {{ display: inline-block; padding: 12px 24px; background: #c09a6c; color: white; text-decoration: none; border-radius: 5px; margin: 20px 0; }}
                        .footer {{ text-align: center; padding: 20px; font-size: 12px; color: #666; }}
                    </style>
                </head>
                <body>
                    <div class='container'>
                        <div class='header'>
                            <h2 style='color: white; margin: 0;'>Furniture Garden Design</h2>
                        </div>
                        <div class='content'>
                            <h3>Password Reset Request</h3>
                            <p>Hello {user.FirstName} {user.LastName},</p>
                            <p>We received a request to reset your password. Click the button below to create a new password:</p>
                            <div style='text-align: center;'>
                                <a href='{resetLink}' class='button' style='color: white;'>Reset Password</a>
                            </div>
                            <p>If the button doesn't work, copy and paste this link into your browser:</p>
                            <p><a href='{resetLink}'>{resetLink}</a></p>
                            <p>This link will expire in 24 hours.</p>
                            <p>If you didn't request this, please ignore this email.</p>
                        </div>
                        <div class='footer'>
                            <p>&copy; 2024 Furniture Garden Design. All rights reserved.</p>
                        </div>
                    </div>
                </body>
                </html>";
        }
    }
}