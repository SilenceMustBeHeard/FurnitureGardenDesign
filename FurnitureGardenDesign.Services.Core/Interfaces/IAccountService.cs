using FurnitureGardenDesign.Web.ViewModels.Account;
using System;
using System.Collections.Generic;
using System.Text;

namespace FurnitureGardenDesign.Services.Core.Interfaces
{
    public interface IAccountService
    {
        Task<(bool Success, IEnumerable<string> Errors)> RegisterAsync(RegisterViewModel model);

        Task<bool> LoginAsync(LoginViewModel model);

        Task LogoutAsync();
    }
}
