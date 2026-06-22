using FurnitureGardenDesign.Data.Models;
using FurnitureGardenDesign.Web.ViewModels.Account;

namespace FurnitureGardenDesign.Services.Core.Interfaces.Account;

public interface IAccountService
{
    Task<(bool Success, string[] Errors)> RegisterAsync(RegisterViewModel model);

    Task<bool> LoginAsync(LoginViewModel model);

    Task LogoutAsync();

    Task<bool> ForgotPasswordAsync(string email, string resetLink);

    Task<(bool Success, string[] Errors)> ResetPasswordAsync(ResetPasswordViewModel model);

    Task<(bool Success, string? Error, AppUser? User)> ConfirmEmailAsync(string userId, string token);

    Task<(bool Success, string? Error, string? ConfirmationLink)> GenerateEmailConfirmationAsync(AppUser user);
}