using FurnitureGardenDesign.Data.Models;
using FurnitureGardenDesign.Data.Repository.Interfaces;
using FurnitureGardenDesign.Services.Core.Interfaces;
using FurnitureGardenDesign.Web.ViewModels.User;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace FurnitureGardenDesign.Services.Core.Implementations
{
    public class ProfileService : IProfileService
    {
        // seed repositories
        private readonly IAppUserRepository _userRepository;
        private readonly UserManager<AppUser> _userManager;


        public ProfileService(
            IAppUserRepository userRepository,
            UserManager<AppUser> userManager)
        {
            _userRepository = userRepository;
            _userManager = userManager;
        }




        // gets the profile data for the user, including their inbox messages
        // THIS WORKS FOR BOTH REGULAR USERS AND ADMINS/MANAGERS!
        public async Task<ProfileViewModel?> GetProfileAsync(string userId)
        {
            var model = await _userRepository
                .GetAllAttached()
                .Where(u => u.Id == userId)
                .Select(u => new ProfileViewModel
                {
                    Id = u.Id,
                    Email = u.Email!,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    Address = u.Address,

                    Inbox = u.InboxMessages
                        .Where(x => x.DesignVariant != null && !x.DesignVariant.IsDeleted)
                        .OrderByDescending(x => x.CreatedOn)
                        .Select(x => new InboxMessageViewModel
                        {
                            Id = x.Id,
                            DesignVariantId = x.DesignVariant!.Id,
                            DesignImage2DUrl = x.DesignVariant!.Image2DUrl,
                            Model3DUrl = x.DesignVariant.Model3DUrl,
                            Notes = x.DesignVariant.Notes,
                            IsRead = x.IsRead,
                            CreatedOn = x.CreatedOn,
                            Type = x.Type 
                        }).ToList()
                })
                .FirstOrDefaultAsync();

            return model;
        }

      

        private async Task<HashSet<string>> GetAllAdminAndManagerIds()
        {
            var adminIds = new HashSet<string>();
            var allUsers = await _userRepository.GetAllAttached().ToListAsync();

            foreach (var user in allUsers)
            {
                var roles = await _userManager.GetRolesAsync(user);
                if (roles.Contains("Admin") || roles.Contains("Manager"))
                {
                    adminIds.Add(user.Id);
                }
            }

            return adminIds;
        }
    }
}