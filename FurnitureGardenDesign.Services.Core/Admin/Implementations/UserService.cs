using FurnitureGardenDesign.Data.Models;
using FurnitureGardenDesign.Services.Core.Admin.Interfaces;
using FurnitureGardenDesign.Web.ViewModels.Admin;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FurnitureGardenDesign.Services.Core.Admin.Implementations
{
    public class UserService : IUserService
    {
        private readonly UserManager<AppUser> _userManager;

        public UserService(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }

        // Get all users with the role user except the  admin
        public async Task<IEnumerable<UserManagmentIndexViewModel>> GetUserManagmentBoardDataAsync(Guid adminId)
        {
            var allUsers = await _userManager.Users.ToListAsync();
            var result = new List<UserManagmentIndexViewModel>();

            foreach (var user in allUsers)
            {
                // skip the  admin
                if (user.Id == adminId.ToString())
                    continue;

                var roles = await _userManager.GetRolesAsync(user);

                // all roles except "Admin"
                result.Add(new UserManagmentIndexViewModel
                    {
                        Id = Guid.Parse(user.Id),  // <-- convert back to Guid if needed
                        Email = user.Email!,
                        Roles = roles
                    });
                
            }

            return result;
        }


        // Change user role (starts from "User")
        public async Task<(bool Failed, string ErrorMessage)> ChangeUserRoleAsync(
     ChangeUserRoleViewModel model,
     Guid adminId)
        {
            // find user by Guid
            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == model.UserId.ToString());
            if (user == null)
                return (true, "User not found.");

            var roles = await _userManager.GetRolesAsync(user);



            // remove existing roles
            var removeResult = await _userManager.RemoveFromRolesAsync(user, roles);
            if (!removeResult.Succeeded)
                return (true, "Failed to remove existing roles.");

            // add new role
            var addResult = await _userManager.AddToRoleAsync(user, model.NewRole);
            if (!addResult.Succeeded)
                return (true, "Failed to assign new role.");

            return (false, string.Empty);
        }

    }
}
