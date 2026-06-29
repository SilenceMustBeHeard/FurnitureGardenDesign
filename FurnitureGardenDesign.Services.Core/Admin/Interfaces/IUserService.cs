using FurnitureGardenDesign.Web.ViewModels.Admin;

namespace FurnitureGardenDesign.Services.Core.Admin.Interfaces
{
    public interface IUserService
    {
        Task<IEnumerable<UserManagmentIndexViewModel>> GetUserManagmentBoardDataAsync(Guid userId);

        Task<UserManagmentIndexViewModel> FindUserByIdAsync(string userId);

        Task<(bool Failed, string ErrorMessage)> DisableUser(string userId);

        Task<(bool Failed, string ErrorMessage)> ChangeUserRoleAsync(
            ChangeUserRoleViewModel model,
            Guid adminId);
    }
}