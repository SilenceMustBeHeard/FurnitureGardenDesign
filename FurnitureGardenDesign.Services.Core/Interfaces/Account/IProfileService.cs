using FurnitureGardenDesign.Web.ViewModels.User;

namespace FurnitureGardenDesign.Services.Core.Interfaces.Account
{
    public interface IProfileService
    {
        Task<ProfileViewModel?> GetProfileAsync(string userId);
    }
}